using VotingAPI.Models.DTOs.Vote;

namespace VotingAPI.Services.Interfaces
{
    public interface IVoteService
    {
        Task<string> SendVoteOtp(Guid userId, VotePrepareRequestDTO votePrepareRequestDTO);

        Task<VotePrepareResponseDTO> PrepareVote(Guid userId, VotePrepareRequestDTO votePrepareRequestDTO);

        Task<(long Nonce, string RegisteredAddress)> GetVoterNonce(Guid userId, Guid electionId);
    }
}
