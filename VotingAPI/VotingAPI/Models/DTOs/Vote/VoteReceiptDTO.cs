namespace VotingAPI.Models.DTOs.Vote
{
    public class VoteReceiptDTO
    {
        public string TxHash { get; set; } = string.Empty;
        public long? BlockNumber { get; set; }
        public DateTime VotedAt { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string ElectionTitle { get; set; } = string.Empty;
        public string ContractAddress { get; set; } = string.Empty;
    }
}
