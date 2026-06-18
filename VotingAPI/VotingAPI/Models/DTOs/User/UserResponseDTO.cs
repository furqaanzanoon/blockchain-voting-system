using VotingAPI.Models.Enums;

namespace VotingAPI.Models.DTOs.User
{
    public class UserResponseDTO
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? EthAddress { get; set; }
        public UserRole Role { get; set; }
        public bool IsApproved { get; set; }
        public bool IsVerified { get; set; }
        public string? PartyAffiliation { get; set; }
    }
}
