namespace VotingAPI.Models.DTOs.Voter
{
    public class VoterResponseDTO
    {
        public Guid UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? EthAddress { get; set; }

        public DateTime RegisteredAt { get; set; }

        public bool HasVoted { get; set; }
    }
}