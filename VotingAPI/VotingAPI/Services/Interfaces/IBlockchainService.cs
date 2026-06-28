using VotingAPI.Models.DTOs.Result;

namespace VotingAPI.Services.Interfaces
{
    public interface IBlockchainService
    {
        Task<string> CreateNewElectionAsync(string title, List<string> candidateNames);
        Task StartVotingAsync(string votingContractAddress);
        Task EndVotingAsync(string votingContractAddress);
        Task GrantBallotAdminRoleAsync(string votingContractAddress);
        Task SetEligibilityAsync(string votingContractAddress, string walletAddress);
        Task<List<CandidateResultDTO>> GetResultsAsync(string votingContractAddress);
        Task<long> GetVoterNonceAsync(string votingContractAddress, string voterAddress);
        Task<(string TxHash, long BlockNumber)> CastVoteWithSignatureAsync(string votingContractAddress, string voterAddress, int candidateIndex, long nonce, string signature);
    }
}