using System.ComponentModel.DataAnnotations;

namespace VotingAPI.Models.DTOs.Vote
{
    public class ConfirmVoteDTO
    {
        [Required(ErrorMessage = "Election Id is required")]
        public Guid ElectionId { get; set; }

        [Required(ErrorMessage = "Candidate Id is required")]
        public Guid CandidateId { get; set; }

        [Required(ErrorMessage = "Transaction Hash is required")]
        public string TxHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Block number is required")]
        public long BlockNumber { get; set; }
    }
}