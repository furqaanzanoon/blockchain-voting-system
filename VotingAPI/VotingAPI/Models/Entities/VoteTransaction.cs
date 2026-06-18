using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VotingAPI.Models.Entities
{
    public class VoteTransaction
    {
        [Key]
        public Guid VoteId { get; set; }

        [Required]
        [MaxLength(66)]
        public string TxHash { get; set; } = string.Empty;   // blockchain tx hash (0x + 64 chars)

        public long? BlockNumber { get; set; }

        public DateTime VotedAt { get; set; }

        // Foreign keys
        public Guid ElectionId { get; set; }
        public Guid CandidateId { get; set; }

        // Navigation properties
        [ForeignKey("ElectionId")]
        public Election Election { get; set; } = null!;

        [ForeignKey("CandidateId")]
        public Candidate Candidate { get; set; } = null!;
    }
}