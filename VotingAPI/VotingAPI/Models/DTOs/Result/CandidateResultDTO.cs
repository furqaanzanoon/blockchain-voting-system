using System.ComponentModel.DataAnnotations;

namespace VotingAPI.Models.DTOs.Result
{
    public class CandidateResultDTO
    {
        [Required(ErrorMessage = "Candidate name is required")]
        public string CandidateName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vote count is required")]
        public long VoteCount { get; set; }
    }
}