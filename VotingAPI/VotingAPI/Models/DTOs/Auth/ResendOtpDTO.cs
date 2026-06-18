using System.ComponentModel.DataAnnotations;

namespace VotingAPI.Models.DTOs.Auth
{
    public class ResendOtpDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
    }
}