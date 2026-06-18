namespace VotingAPI.Models.DTOs.Election
{
    public class CandidateResponseDTO
    {
        public Guid CandidateId { get; set; }
        public string CandidateName { get; set; } = null!;
        public string? PartyAffiliation { get; set; }
        public string? Description { get; set; }
        public bool IsApproved { get; set; }
        public int? OnChainIndex { get; set; }
    }
}