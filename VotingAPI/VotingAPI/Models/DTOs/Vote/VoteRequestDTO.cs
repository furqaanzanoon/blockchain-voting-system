using System.ComponentModel.DataAnnotations;

namespace VotingAPI.Models.DTOs.Vote
{
    public class VoteRequestDTO
    {
        [Required(ErrorMessage = "Election Id is required")]
        public Guid ElectionId { get; set; }

        [Required(ErrorMessage = "Candidate Id is required")]
        public Guid CandidateId { get; set; }
    }
}