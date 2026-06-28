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
            {
                return (0, voterAddress);
            }

            var nonce = await blockchainService.GetVoterNonceAsync(election.ContractAddress, voterAddress);
            return (nonce, voterAddress);
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

    }
}
