using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VotingAPI.Models.Entities
{
    public class Voter
    {
        [Key]
        public Guid VoterId { get; set; }

        public bool HasVoted { get; set; }     // off-chain mirror of on-chain state

        [MaxLength(66)]
        public string? TxHash { get; set; }

        public long? BlockNumber { get; set; }

        public DateTime RegisteredAt { get; set; }

        // Foreign keys
        public Guid ElectionId { get; set; }
        public Guid UserId { get; set; }

        // Navigation properties
        [ForeignKey("ElectionId")]
        public Election Election { get; set; } = null!;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}