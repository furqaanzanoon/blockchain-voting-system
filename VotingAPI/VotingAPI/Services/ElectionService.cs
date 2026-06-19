using Microsoft.EntityFrameworkCore;
using VotingAPI.Data;
using VotingAPI.Models.DTOs.Election;
using VotingAPI.Models.Entities;
using VotingAPI.Models.Enums;
using VotingAPI.Services.Interfaces;

namespace VotingAPI.Services
{
    public class ElectionService : IElectionService
    {
        private readonly VotingDbContext dbContext;
        private readonly IBlockchainService blockchainService;

        public ElectionService(VotingDbContext dbContext, IBlockchainService blockchainService)
        {
            this.dbContext = dbContext;
            this.blockchainService = blockchainService;
        }

        public async Task<List<ElectionResponseDTO>> GetAllElections(Guid? currentUserId = null)
        {
            var query = dbContext.Elections.AsNoTracking();

            List<ElectionResponseDTO> elections;

            if (currentUserId.HasValue)
            {
                elections = await query
                    .GroupJoin(
                        dbContext.Voters.Where(v => v.UserId == currentUserId.Value),
                        e => e.ElectionId,
                        v => v.ElectionId,
                        (e, voters) => new { Election = e, Voters = voters })
                    .SelectMany(
                        x => x.Voters.DefaultIfEmpty(),
                        (x, voter) => new ElectionResponseDTO
                        {
                            ElectionId = x.Election.ElectionId,
                            Title = x.Election.Title,
                            Description = x.Election.Description ?? string.Empty,
                            StartTime = x.Election.StartTime,
                            EndTime = x.Election.EndTime,
                            Status = x.Election.Status,
                            AutoActivate = x.Election.AutoActivate,
                            AutoActivateFailReason = x.Election.AutoActivateFailReason,
                            AutoClose = x.Election.AutoClose,
                            HasVoted = voter != null && voter.HasVoted,
                            CandidateCount = dbContext.Candidates.Count(c => c.ElectionId == x.Election.ElectionId && c.IsApproved),
                            ContractAddress = x.Election.ContractAddress
                        })
                    .ToListAsync();
            }
            else
            {
                elections = await query.Select(e => new ElectionResponseDTO
                {
                    ElectionId = e.ElectionId,
                    Title = e.Title,
                    Description = e.Description ?? string.Empty,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    Status = e.Status,
                    AutoActivate = e.AutoActivate,
                    AutoActivateFailReason = e.AutoActivateFailReason,
                    AutoClose = e.AutoClose,
                    HasVoted = false,
                    CandidateCount = dbContext.Candidates.Count(c => c.ElectionId == e.ElectionId && c.IsApproved),
                    ContractAddress = e.ContractAddress
                }).ToListAsync();
            }

            return elections;
        }

        public async Task<ElectionResponseDTO> GetElectionById(Guid electionId)
        {
            var election = await dbContext.Elections.AsNoTracking().FirstOrDefaultAsync(e => e.ElectionId == electionId) ?? throw new KeyNotFoundException("Election not found");

            var electionResponse = new ElectionResponseDTO
            {
                ElectionId = election.ElectionId,
                Title = election.Title,
                Description = election.Description ?? string.Empty,
                StartTime = election.StartTime,
                EndTime = election.EndTime,
                Status = election.Status,
                AutoActivate = election.AutoActivate,
                AutoActivateFailReason = election.AutoActivateFailReason,
                AutoClose = election.AutoClose,
                CandidateCount = await dbContext.Candidates.CountAsync(c => c.ElectionId == electionId),
                ContractAddress = election.ContractAddress
            };

            return electionResponse;
        }

        public async Task<string> CreateElection(CreateElectionDTO createElectionDTO, Guid createdBy)
        {
            var exists = await dbContext.Elections.AnyAsync(e => e.Title == createElectionDTO.Title);

            if (exists)
                throw new InvalidOperationException("Election already exists");

            if (createElectionDTO.StartTime >= createElectionDTO.EndTime)
                throw new ArgumentException("Invalid election time range");

            if (createElectionDTO.StartTime < DateTime.UtcNow)
                throw new ArgumentException("Start Time cannot be in the past");

            if (createElectionDTO.EndTime < DateTime.UtcNow)
                throw new ArgumentException("End Time cannot be in the past");

            var election = new Election
            {
                Title = createElectionDTO.Title,
                Description = createElectionDTO.Description,
                StartTime = createElectionDTO.StartTime,
                EndTime = createElectionDTO.EndTime,
                Status = ElectionStatus.Draft,
                AutoActivate = createElectionDTO.AutoActivate,
                AutoClose = createElectionDTO.AutoClose,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            await dbContext.AddAsync(election);
            await dbContext.SaveChangesAsync();

            return "Election created successfully";
        }

        public async Task<string> ActivateElection(Guid electionId)
        {
            var election = await dbContext.Elections.Include(e => e.Candidates).Include(e => e.Voters).ThenInclude(v => v.User).FirstOrDefaultAsync(e => e.ElectionId == electionId) ?? throw new KeyNotFoundException("Election not found");

            if (election.Status != ElectionStatus.Draft)
                throw new InvalidOperationException("Only draft elections can be activated");

            var candidateList = election.Candidates.Where(c => c.IsApproved).OrderBy(c => c.CreatedAt).ToList();

            if (candidateList.Count < 2)
                throw new ArgumentException("At least 2 approved candidates required to activate the election");

            if (election.StartTime > DateTime.UtcNow)
                throw new ArgumentException("Voting cannot be started before its scheduled start time");

            if (election.EndTime <= DateTime.UtcNow)
                throw new ArgumentException("Cannot activate already expired election");

            var candidates = candidateList.Select(c => c.Name).ToList();

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var votingContractAddress = await blockchainService.CreateNewElectionAsync(election.Title, candidates);

                for (int i = 0; i < candidateList.Count; i++)
                {
                    candidateList[i].OnChainIndex = i;
                }

                await blockchainService.GrantBallotAdminRoleAsync(votingContractAddress);

                // Set voter eligibility sequentially to avoid admin-wallet nonce collisions
                var eligibleVoters = election.Voters
                    .Where(v => !string.IsNullOrEmpty(v.User.EthAddress))
                    .ToList();

                foreach (var voter in eligibleVoters)
                {
                    await blockchainService.SetEligibilityAsync(votingContractAddress, voter.User.EthAddress!);
                }

                election.ContractAddress = votingContractAddress;
                election.Status = ElectionStatus.Active;
                await dbContext.SaveChangesAsync();

                await blockchainService.StartVotingAsync(votingContractAddress);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return "Election activated successfully";
        }

        public async Task<string> CloseElection(Guid electionId)
        {
            var election = await dbContext.Elections.FirstOrDefaultAsync(e => e.ElectionId == electionId) ?? throw new KeyNotFoundException("Election not found");

            if (election.Status != ElectionStatus.Active)
                throw new InvalidOperationException("Only active elections can be closed");

            if (election.EndTime > DateTime.UtcNow)
                throw new InvalidOperationException("Election can't be closed before end time");

            var votingContractAddress = election.ContractAddress ?? throw new KeyNotFoundException("Election does not have a contract address");

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                await blockchainService.EndVotingAsync(votingContractAddress);

                election.Status = ElectionStatus.Closed;
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return "Election closed successfully";
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task<string> AddCandidate(Guid electionId, AddCandidateDTO addCandidateDTO, UserRole currentRole, string callerName)
        {
            var election = await dbContext.Elections.FirstOrDefaultAsync(e => e.ElectionId == electionId) ?? throw new KeyNotFoundException("Election not found");

            if (election.Status != ElectionStatus.Draft)
                throw new InvalidOperationException("Candidates can only be added to draft elections");

            var candidateExists = await dbContext.Candidates.AnyAsync(c => c.ElectionId == electionId && c.Name == addCandidateDTO.Name);

            if (candidateExists)
                throw new InvalidOperationException("Candidate already exists");

            bool isApproved = false;

            if (currentRole == UserRole.Admin || currentRole == UserRole.ElectionOfficer)
            {
                isApproved = true;
            }
            else if (currentRole == UserRole.Party)
            {
                if (addCandidateDTO.PartyAffiliation != callerName)
                {
                    throw new UnauthorizedAccessException("You can only apply candidates affiliated with your own party.");
                }
                isApproved = false;
            }
            else if (currentRole == UserRole.Candidate)
            {
                if (addCandidateDTO.Name != callerName)
                {
                    throw new UnauthorizedAccessException("Candidates can only apply for themselves.");
                }
                if (addCandidateDTO.PartyAffiliation != "Independent")
                {
                    throw new UnauthorizedAccessException("As a candidate user, you must apply as 'Independent' if applying yourself. Party-affiliated candidates must be applied by their respective political party.");
                }
                isApproved = false;
            }
            else
            {
                throw new UnauthorizedAccessException("Unauthorized role for adding/applying candidates.");
            }

            var candidate = new Candidate
            {
                Name = addCandidateDTO.Name,
                PartyAffiliation = addCandidateDTO.PartyAffiliation,
                Description = addCandidateDTO.Description,
                IsApproved = isApproved,
                CreatedAt = DateTime.UtcNow,
                ElectionId = electionId
            };

            await dbContext.Candidates.AddAsync(candidate);
            await dbContext.SaveChangesAsync();

            return isApproved ? "Candidate added successfully" : "Candidate application submitted successfully and is pending approval";
        }

        public async Task<string> RemoveCandidate(Guid electionId, Guid candidateId)
        {
            var election = await dbContext.Elections.FirstOrDefaultAsync(e => e.ElectionId == electionId) ?? throw new KeyNotFoundException("Election not found");

            if (election.Status != ElectionStatus.Draft)
                throw new InvalidOperationException("Candidates can only be removed from draft elections");

            var candidate = await dbContext.Candidates.FirstOrDefaultAsync(c => c.CandidateId == candidateId && c.ElectionId == electionId) ?? throw new KeyNotFoundException("Candidate not found");

            dbContext.Candidates.Remove(candidate);
            await dbContext.SaveChangesAsync();

            return "Candidate removed successfully";
        }

        public async Task<List<CandidateResponseDTO>> GetCandidates(Guid electionId, bool includeUnapproved)
        {
            var electionExists = await dbContext.Elections.AnyAsync(e => e.ElectionId == electionId);

            if (!electionExists)
                throw new KeyNotFoundException("Election not found");

            var query = dbContext.Candidates.Where(c => c.ElectionId == electionId);
            if (!includeUnapproved)
            {
                query = query.Where(c => c.IsApproved);
            }

            var candidates = await query.OrderBy(c => c.CreatedAt).Select(c => new CandidateResponseDTO
            {
                CandidateId = c.CandidateId,
                CandidateName = c.Name,
                Description = c.Description,
                PartyAffiliation = c.PartyAffiliation,
                IsApproved = c.IsApproved,
                OnChainIndex = c.OnChainIndex
            }).ToListAsync();

            return candidates;
        }

        public async Task<string> ApproveCandidate(Guid electionId, Guid candidateId)
        {
            var election = await dbContext.Elections.FirstOrDefaultAsync(e => e.ElectionId == electionId) ?? throw new KeyNotFoundException("Election not found");

            if (election.Status != ElectionStatus.Draft)
                throw new InvalidOperationException("Candidates can only be approved in draft elections");

            var candidate = await dbContext.Candidates.FirstOrDefaultAsync(c => c.CandidateId == candidateId && c.ElectionId == electionId) ?? throw new KeyNotFoundException("Candidate not found");

            candidate.IsApproved = true;
            await dbContext.SaveChangesAsync();

            return "Candidate approved successfully";
        }
    }
}