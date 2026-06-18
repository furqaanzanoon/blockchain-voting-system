using System.ComponentModel.DataAnnotations;

namespace VotingAPI.Models.DTOs.Election
{
    public class AddCandidateDTO
    {
        [Required(ErrorMessage = "Candidate name is required.")]
        public string Name { get; set; } = null!;

        public string? PartyAffiliation { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}