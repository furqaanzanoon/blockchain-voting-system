using System.ComponentModel.DataAnnotations;

namespace VotingAPI.Models.DTOs.Election
{
    public class CreateElectionDTO
    {
        [Required(ErrorMessage = "Election title is required.")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        public DateTime EndTime { get; set; }

        public bool AutoActivate { get; set; }
        public bool AutoClose { get; set; }
    }

    public class UpdateMerkleRootDTO
    {
        [Required]
        public string MerkleRoot { get; set; } = string.Empty;
    }
}