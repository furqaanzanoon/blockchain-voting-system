namespace VotingAPI.Models.DTOs.Vote
{
    public class VoteResponseDTO
    {
        public string TransactionHash { get; set; } = null!;
        public long BlockNumber { get; set; }
    }
}