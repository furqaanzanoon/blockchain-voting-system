using System.ComponentModel.DataAnnotations;

namespace VotingAPI.Models.DTOs.Voter
{
    public class RegisterVoterDTO
    {
        [Required(ErrorMessage = "Election Id is required")]
        public Guid ElectionId { get; set; }

        [Required(ErrorMessage = "User Id is required")]
        public Guid UserId { get; set; }
    }
}