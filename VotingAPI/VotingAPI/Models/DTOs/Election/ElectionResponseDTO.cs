using VotingAPI.Models.Enums;

namespace VotingAPI.Models.DTOs.Election
{
    public class ElectionResponseDTO
    {
        public Guid ElectionId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ElectionStatus Status { get; set; }
        public bool AutoActivate { get; set; }
        public string? AutoActivateFailReason { get; set; }
        public bool AutoClose { get; set; }
        public bool HasVoted { get; set; }
        public int CandidateCount { get; set; }
        public string? ContractAddress { get; set; }
    }
}