using Microsoft.EntityFrameworkCore;
using VotingAPI.Data;
using VotingAPI.Helpers;
using VotingAPI.Models.DTOs.Vote;
using VotingAPI.Models.Entities;
using VotingAPI.Models.Enums;
using VotingAPI.Services.Interfaces;

namespace VotingAPI.Services
{
    public class VoteService : IVoteService
    {
        private readonly VotingDbContext dbContext;
        private readonly IEmailService emailService;
        private readonly IBlockchainService blockchainService;

        public VoteService(VotingDbContext dbContext, IEmailService emailService, IBlockchainService blockchainService)
        {
            this.dbContext = dbContext;
            this.emailService = emailService;
            this.blockchainService = blockchainService;
        }

        public async Task<string> SendVoteOtp(Guid userId, VotePrepareRequestDTO votePrepareRequestDTO)
        {
            await ValidateVoteRequest(userId, votePrepareRequestDTO);

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId) ?? throw new KeyNotFoundException("User not found");
            var otp = EmailHelper.GetOtp();
            var body = EmailHelper.GetBody(user.FullName, otp);

            user.OtpCode = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);

            await dbContext.SaveChangesAsync();
            await emailService.SendEmailAsync(user.Email, "Vote OTP Verification", body);

            return "OTP sent to your registered email.";
        }

        public async Task<VotePrepareResponseDTO> PrepareVote(Guid userId, VotePrepareRequestDTO votePrepareRequestDTO)
        {
            var election = await ValidateVoteRequest(userId, votePrepareRequestDTO);
            await ValidateVoteOtp(userId, votePrepareRequestDTO.Otp);
            var candidate = election.Candidates.First(c => c.CandidateId == votePrepareRequestDTO.CandidateId);

            if (string.IsNullOrWhiteSpace(votePrepareRequestDTO.Signature))
            {
                throw new ArgumentException("Vote cryptographic signature is required.");
            }

            // Get voter's wallet address
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId) ?? throw new KeyNotFoundException("User not found");
            var voterAddress = user.EthAddress ?? throw new InvalidOperationException("Wallet not connected. Please connect your wallet before voting.");

            // Cast vote on-chain via admin wallet using voter's EIP-712 signature (gasless & trustless)
            var (txHash, blockNumber) = await blockchainService.CastVoteWithSignatureAsync(
                election.ContractAddress!,
                voterAddress,
                candidate.OnChainIndex!.Value,
                votePrepareRequestDTO.Nonce,
                votePrepareRequestDTO.Signature
            );

            // Record the vote in the database inside a transaction to ensure consistency
            var voter = await dbContext.Voters.FirstOrDefaultAsync(v => v.UserId == userId && v.ElectionId == votePrepareRequestDTO.ElectionId) ?? throw new KeyNotFoundException("Voter not found");

            var votedAt = DateTime.UtcNow;
            var voteTransaction = new VoteTransaction
            {
                TxHash = txHash,
                BlockNumber = blockNumber,
                VotedAt = votedAt,
                ElectionId = votePrepareRequestDTO.ElectionId,
                CandidateId = votePrepareRequestDTO.CandidateId
            };

            using var dbTransaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                voter.HasVoted = true;
                voter.TxHash = txHash;
                voter.BlockNumber = blockNumber;
                await dbContext.VoteTransactions.AddAsync(voteTransaction);
                await dbContext.SaveChangesAsync();
                await dbTransaction.CommitAsync();
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }

            return new VotePrepareResponseDTO
            {
                ContractAddress = election.ContractAddress!,
                CandidateIndex = candidate.OnChainIndex!.Value,
                TxHash = txHash,
                BlockNumber = blockNumber
            };
        }

        public async Task<(long Nonce, string RegisteredAddress)> GetVoterNonce(Guid userId, Guid electionId)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId) ?? throw new KeyNotFoundException("User not found");
            var voterAddress = user.EthAddress ?? throw new InvalidOperationException("Wallet not connected. Please connect your wallet first.");

            var election = await dbContext.Elections.FirstOrDefaultAsync(e => e.ElectionId == electionId) ?? throw new KeyNotFoundException("Election not found");
            if (string.IsNullOrWhiteSpace(election.ContractAddress))
                throw new InvalidOperationException("Election not deployed on chain");

            var nonce = await blockchainService.GetVoterNonceAsync(election.ContractAddress, voterAddress);
            return (nonce, voterAddress);
        }

        public async Task<VoteReceiptDTO?> GetVoteReceipt(Guid userId, Guid electionId)
        {
            var voter = await dbContext.Voters
                .Include(v => v.Election)
                .FirstOrDefaultAsync(v => v.UserId == userId && v.ElectionId == electionId);

            if (voter == null || !voter.HasVoted || string.IsNullOrEmpty(voter.TxHash))
                return null;

            // Use the actual vote timestamp from the VoteTransaction table (not registration time)
            var voteTransaction = await dbContext.VoteTransactions
                .FirstOrDefaultAsync(vt => vt.TxHash == voter.TxHash && vt.ElectionId == electionId);

            return new VoteReceiptDTO
            {
                TxHash = voter.TxHash,
                BlockNumber = voter.BlockNumber,
                VotedAt = voteTransaction?.VotedAt ?? voter.RegisteredAt,
                CandidateName = "Anonymous (Ballot Secret)",
                ElectionTitle = voter.Election.Title,
                ContractAddress = voter.Election.ContractAddress ?? string.Empty
            };
        }

        private async Task<Election> ValidateVoteRequest(Guid userId, VotePrepareRequestDTO votePrepareRequestDTO)
        {
            var voter = await dbContext.Voters
                .Include(v => v.Election)
                .ThenInclude(e => e.Candidates)
                .FirstOrDefaultAsync(v => v.UserId == userId && v.ElectionId == votePrepareRequestDTO.ElectionId);

            if (voter == null)
            {
                var electionExists = await dbContext.Elections.AnyAsync(e => e.ElectionId == votePrepareRequestDTO.ElectionId);
                if (!electionExists)
                    throw new KeyNotFoundException("Election not found");
                throw new InvalidOperationException("Not registered for election");
            }
            
            var election = voter.Election;

            if (election.Status != ElectionStatus.Active)
                throw new InvalidOperationException("Election is not active");

            if (election.EndTime <= DateTime.UtcNow)
                throw new InvalidOperationException("Election has ended");

            var candidate = election.Candidates.FirstOrDefault(c => c.CandidateId == votePrepareRequestDTO.CandidateId) ?? throw new KeyNotFoundException("Candidate not found");

            if (candidate.OnChainIndex == null)
                throw new InvalidOperationException("Candidate not mapped on chain");

            var alreadyVoted = voter.HasVoted;

            if (alreadyVoted)
                throw new InvalidOperationException("Already voted");

            return election;
        }

        private async Task ValidateVoteOtp(Guid userId, string? otp)
        {
            if (string.IsNullOrWhiteSpace(otp))
                throw new ArgumentException("Vote OTP is required.");

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId) ?? throw new KeyNotFoundException("User not found");

            if (user.OtpCode != otp)
                throw new ArgumentException("Invalid vote OTP.");

            if (user.OtpExpiry == null || user.OtpExpiry < DateTime.UtcNow)
                throw new ArgumentException("Vote OTP has expired.");

            user.OtpCode = null;
            user.OtpExpiry = null;

            await dbContext.SaveChangesAsync();
        }

        public async Task ConfirmVote(Guid userId, ConfirmVoteDTO confirmVoteDTO)
        {
            var voter = await dbContext.Voters.FirstOrDefaultAsync(v => v.UserId == userId && v.ElectionId == confirmVoteDTO.ElectionId) ?? throw new KeyNotFoundException("Voter not found");

            var voterExists = voter.HasVoted;

            if (voterExists)
                throw new InvalidOperationException("Vote already recorded");

            var candidateExists = await dbContext.Candidates.AnyAsync(c => c.CandidateId == confirmVoteDTO.CandidateId && c.ElectionId == confirmVoteDTO.ElectionId);

            if (!candidateExists)
                throw new KeyNotFoundException("Candidate not found");

            var voteTransaction = new VoteTransaction
            {
                TxHash = confirmVoteDTO.TxHash,
                BlockNumber = confirmVoteDTO.BlockNumber,
                VotedAt = DateTime.UtcNow,
                ElectionId = confirmVoteDTO.ElectionId,
                CandidateId = confirmVoteDTO.CandidateId
            };

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                voter.HasVoted = true;
                voter.TxHash = confirmVoteDTO.TxHash;
                voter.BlockNumber = confirmVoteDTO.BlockNumber;
                await dbContext.VoteTransactions.AddAsync(voteTransaction);
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            
        }

        public async Task<(string TxHash, long BlockNumber)> CastZkVoteAsync(Guid userId, CastZkVoteDTO dto)
        {
            var voter = await dbContext.Voters
                .Include(v => v.Election)
                .FirstOrDefaultAsync(v => v.UserId == userId && v.ElectionId == dto.ElectionId) ?? throw new KeyNotFoundException("Voter registration not found");

            if (voter.Election.Status != ElectionStatus.Active)
                throw new InvalidOperationException("Election is not active");

            if (voter.Election.EndTime <= DateTime.UtcNow)
                throw new InvalidOperationException("Election has ended");

            if (voter.HasVoted)
                throw new InvalidOperationException("Already voted");

            await ValidateVoteOtp(userId, dto.Otp);

            var proof = new VotingAPI.Services.Blockchain.Generated.ZKVerifier.ContractDefinition.Proof
            {
                A = new VotingAPI.Services.Blockchain.Generated.ZKVerifier.ContractDefinition.G1Point
                {
                    X = System.Numerics.BigInteger.Parse(dto.Proof.A.X),
                    Y = System.Numerics.BigInteger.Parse(dto.Proof.A.Y)
                },
                B = new VotingAPI.Services.Blockchain.Generated.ZKVerifier.ContractDefinition.G2Point
                {
                    X = dto.Proof.B.X.Select(x => System.Numerics.BigInteger.Parse(x)).ToList(),
                    Y = dto.Proof.B.Y.Select(y => System.Numerics.BigInteger.Parse(y)).ToList()
                },
                C = new VotingAPI.Services.Blockchain.Generated.ZKVerifier.ContractDefinition.G1Point
                {
                    X = System.Numerics.BigInteger.Parse(dto.Proof.C.X),
                    Y = System.Numerics.BigInteger.Parse(dto.Proof.C.Y)
                }
            };

            var signals = new VotingAPI.Services.Blockchain.Generated.ZKVerifier.ContractDefinition.PublicSignals
            {
                MerkleRoot = System.Numerics.BigInteger.Parse(dto.PublicSignals.MerkleRoot),
                NullifierHash = System.Numerics.BigInteger.Parse(dto.PublicSignals.NullifierHash),
                BallotId = System.Numerics.BigInteger.Parse(dto.PublicSignals.BallotId),
                VoteCommitment = System.Numerics.BigInteger.Parse(dto.PublicSignals.VoteCommitment)
            };

            // Cast vote on-chain via ZKVerifier using Admin wallet
            var (txHash, blockNumber) = await blockchainService.VerifyZkVoteAsync(proof, signals);

            voter.HasVoted = true;
            voter.TxHash = txHash;
            voter.BlockNumber = blockNumber;

            var voteTransaction = new VoteTransaction
            {
                TxHash = txHash,
                BlockNumber = blockNumber,
                VotedAt = DateTime.UtcNow,
                ElectionId = dto.ElectionId,
                CandidateId = dto.CandidateId
            };

            await dbContext.VoteTransactions.AddAsync(voteTransaction);
            await dbContext.SaveChangesAsync();

            return (txHash, blockNumber);
        }

        public async Task<string> RegisterVoterCommitment(Guid electionId, Guid userId, string commitment)
        {
            if (string.IsNullOrWhiteSpace(commitment))
                throw new ArgumentException("Identity commitment is required");

            var voter = await dbContext.Voters
                .Include(v => v.Election)
                .FirstOrDefaultAsync(v => v.UserId == userId && v.ElectionId == electionId) 
                ?? throw new KeyNotFoundException("Voter registration not found for this election");

            if (voter.Election.Status != ElectionStatus.Draft)
                throw new InvalidOperationException("Identity commitment can only be registered during the draft phase");

            voter.IdentityCommitment = commitment;
            await dbContext.SaveChangesAsync();

            return "Voter identity commitment registered successfully";
        }

        public async Task<List<string>> GetElectionVoterCommitments(Guid electionId)
        {
            var electionExists = await dbContext.Elections.AnyAsync(e => e.ElectionId == electionId);
            if (!electionExists)
                throw new KeyNotFoundException("Election not found");

            // We order deterministically by VoterId to build the identical Merkle tree on both frontend and backend
            var commitments = await dbContext.Voters
                .Where(v => v.ElectionId == electionId && !string.IsNullOrEmpty(v.IdentityCommitment))
                .OrderBy(v => v.VoterId)
                .Select(v => v.IdentityCommitment!)
                .ToListAsync();

            return commitments;
        }
    }
}
