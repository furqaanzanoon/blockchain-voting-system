using VotingAPI.Models.DTOs.Result;

namespace VotingAPI.Services.Interfaces
{
    public interface IResultService
    {
        Task<List<CandidateResultDTO>> GetElectionResults(Guid electionId);
        Task<List<VoteAuditResponseDTO>> GetElectionAudit(Guid electionId);
    }
}