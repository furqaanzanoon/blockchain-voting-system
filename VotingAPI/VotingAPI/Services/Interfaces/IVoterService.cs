using VotingAPI.Models.DTOs.Voter;

namespace VotingAPI.Services.Interfaces
{
    public interface IVoterService
    {
        Task<string> RegisterVoter(Guid ElectionId, Guid UserId);
        Task<List<VoterResponseDTO>> GetElectionVoters(Guid electionId);
    }
}