using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VotingAPI.Models.Enums;

namespace VotingAPI.Models.Entities
{
    public class Election
    {
        [Key]
        public Guid ElectionId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(42)]
        public string? ContractAddress { get; set; }    // filled after smart contract deploy

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public ElectionStatus Status { get; set; } // By default, it will be set to "Draft" when the election is created, and will be updated to "Active" when the election starts, and "Closed" when the election ends.

        public bool AutoActivate { get; set; } // If true, the election will be automatically activated when the start time arrives.

        public bool AutoClose { get; set; } // If true, the election will be automatically closed when the end time arrives.

        [MaxLength(500)]
        public string? AutoActivateFailReason { get; set; } // Populated when auto-activation fails (e.g., not enough candidates)

        public DateTime CreatedAt { get; set; }

        [MaxLength(78)]
        public string? MerkleRoot { get; set; }

        // Foreign key
        public Guid CreatedBy { get; set; }

        // Navigation properties
        [ForeignKey("CreatedBy")]
        public User Creator { get; set; } = null!;
        public ICollection<Candidate> Candidates { get; set; } = [];
        public ICollection<Voter> Voters { get; set; } = [];
        public ICollection<VoteTransaction> VoteTransactions { get; set; } = [];
    }
}