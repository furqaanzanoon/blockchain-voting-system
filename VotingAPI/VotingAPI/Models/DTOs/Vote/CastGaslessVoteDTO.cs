namespace VotingAPI.Models.DTOs.Vote
{
    public class CastGaslessVoteDTO
    {
        public Guid ElectionId { get; set; }
        public Guid CandidateId { get; set; }
        public string Otp { get; set; } = null!;
        public string VoterAddress { get; set; } = null!;
        public string Signature { get; set; } = null!;
    }
}
