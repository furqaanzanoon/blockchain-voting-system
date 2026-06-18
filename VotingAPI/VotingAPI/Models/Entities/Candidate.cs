using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VotingAPI.Models.Entities
{
    public class Candidate
    {
        [Key]
        public Guid CandidateId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? PartyAffiliation { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public int? OnChainIndex { get; set; } // index in smart contract (1, 2, 3...)

        public bool IsApproved { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        // Foreign key
        public Guid ElectionId { get; set; }

        // Navigation properties
        [ForeignKey("ElectionId")]
        public Election Election { get; set; } = null!;
        public ICollection<VoteTransaction> VoteTransactions { get; set; } = new List<VoteTransaction>();
    }
}