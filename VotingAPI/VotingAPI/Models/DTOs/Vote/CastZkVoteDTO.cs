using System;
using System.Collections.Generic;

namespace VotingAPI.Models.DTOs.Vote
{
    public class G1PointDTO
    {
        public string X { get; set; } = string.Empty;
        public string Y { get; set; } = string.Empty;
    }

    public class G2PointDTO
    {
        public List<string> X { get; set; } = [];
        public List<string> Y { get; set; } = [];
    }

    public class ProofDTO
    {
        public G1PointDTO A { get; set; } = null!;
        public G2PointDTO B { get; set; } = null!;
        public G1PointDTO C { get; set; } = null!;
    }

    public class PublicSignalsDTO
    {
        public string MerkleRoot { get; set; } = string.Empty;
        public string NullifierHash { get; set; } = string.Empty;
        public string BallotId { get; set; } = string.Empty;
        public string VoteCommitment { get; set; } = string.Empty;
    }

    public class CastZkVoteDTO
    {
        public Guid ElectionId { get; set; }
        public Guid CandidateId { get; set; }
        public string Otp { get; set; } = string.Empty;
        public ProofDTO Proof { get; set; } = null!;
        public PublicSignalsDTO PublicSignals { get; set; } = null!;
    }

    public class RegisterCommitmentDTO
    {
        public Guid ElectionId { get; set; }
        public string Commitment { get; set; } = string.Empty;
    }
}
