using System.ComponentModel.DataAnnotations;
using VotingAPI.Models.Enums;

namespace VotingAPI.Models.Entities
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string? EthAddress { get; set; } // Ethereum wallet address

        [Required]
        public UserRole Role { get; set; }

        public string? PartyAffiliation { get; set; }

        public bool IsVerified { get; set; }

        public bool IsApproved { get; set; }

        public string? OtpCode { get; set; }

        public DateTime? OtpExpiry {  get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<Election> CreatedElections { get; set; } = [];
        public ICollection<Voter> Voters { get; set; } = [];
    }
}