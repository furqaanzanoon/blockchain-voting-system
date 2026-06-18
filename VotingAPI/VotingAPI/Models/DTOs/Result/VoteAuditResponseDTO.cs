namespace VotingAPI.Models.DTOs.Result
{
    public class VoteAuditResponseDTO
    {
        public Guid VoterId { get; set; }

        public string VoterName { get; set; } = null!;

        public Guid CandidateId { get; set; }

        public string CandidateName { get; set; } = null!;

        public string TransactionHash { get; set; } = null!;

        public long BlockNumber { get; set; }

        public DateTime VotedAt { get; set; }
    }
}