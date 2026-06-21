using Microsoft.EntityFrameworkCore;
using VotingAPI.Data;
using VotingAPI.Models.DTOs.Result;
using VotingAPI.Models.Enums;
using VotingAPI.Services.Interfaces;

namespace VotingAPI.Services
{
    public class ResultService : IResultService
    {
        private readonly VotingDbContext dbContext;
        private readonly IBlockchainService blockchainService;

        public ResultService(VotingDbContext dbContext, IBlockchainService blockchainService)
        {
            this.dbContext = dbContext;
            this.blockchainService = blockchainService;
        }

        public async Task<List<CandidateResultDTO>> GetElectionResults(Guid electionId)
        {
            var election = await dbContext.Elections.FirstOrDefaultAsync(e => e.ElectionId == electionId) ?? throw new KeyNotFoundException("Election not found");

            if (election.Status != ElectionStatus.Closed)
                throw new InvalidOperationException("Election is not closed yet");

            if (string.IsNullOrWhiteSpace(election.ContractAddress))
                throw new InvalidOperationException("Election not activated");

            var results = await dbContext.Candidates
                .Where(c => c.ElectionId == electionId)
                .Select(c => new CandidateResultDTO
                {
                    CandidateName = c.Name,
                    VoteCount = dbContext.VoteTransactions.Count(vt => vt.CandidateId == c.CandidateId)
                })
                .OrderByDescending(r => r.VoteCount)
                .ToListAsync();

            return results;
        }

        public async Task<List<VoteAuditResponseDTO>> GetElectionAudit(Guid electionId)
        {
            var election = await dbContext.Elections.FirstOrDefaultAsync(e => e.ElectionId == electionId) ?? throw new KeyNotFoundException("Election not found");

            if (election.Status != ElectionStatus.Closed)
                throw new InvalidOperationException("Audit available only after election closes");

            var auditLogs = await dbContext.VoteTransactions.Where(vt => vt.ElectionId == electionId).OrderBy(vt => vt.VotedAt).Select(vt => new VoteAuditResponseDTO
            {
                VoterId = Guid.Empty,
                VoterName = "Anonymous",
                CandidateId = vt.CandidateId,
                CandidateName = vt.Candidate.Name,
                TransactionHash = vt.TxHash,
                BlockNumber = vt.BlockNumber ?? 0,
                VotedAt = vt.VotedAt
            }).ToListAsync();

            return auditLogs;
        }
    }
}