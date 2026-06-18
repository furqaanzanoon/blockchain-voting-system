using VotingAPI.Models.DTOs.Vote;

namespace VotingAPI.Services.Interfaces
{
    public interface IVoteService
    {
        Task<string> SendVoteOtp(Guid userId, VotePrepareRequestDTO votePrepareRequestDTO);

        Task<VotePrepareResponseDTO> PrepareVote(Guid userId, VotePrepareRequestDTO votePrepareRequestDTO);

        Task ConfirmVote(Guid userId, ConfirmVoteDTO confirmVoteDTO);

        Task<(long Nonce, string RegisteredAddress)> GetVoterNonce(Guid userId, Guid electionId);

        Task<VoteReceiptDTO?> GetVoteReceipt(Guid userId, Guid electionId);

        Task<(string TxHash, long BlockNumber)> CastZkVoteAsync(Guid userId, CastZkVoteDTO castZkVoteDTO);
    }
}
