using System.ComponentModel.DataAnnotations;
using VotingAPI.Models.Enums;

namespace VotingAPI.Models.DTOs.Auth
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;

        public UserRole Role { get; set; }
    }
}