namespace VotingAPI.Models.DTOs.Vote
{
    public class VotePrepareResponseDTO
    {
        public string ContractAddress { get; set; } = string.Empty;

        public int CandidateIndex { get; set; }

        public string TxHash { get; set; } = string.Empty;

        public long BlockNumber { get; set; }
    }
}