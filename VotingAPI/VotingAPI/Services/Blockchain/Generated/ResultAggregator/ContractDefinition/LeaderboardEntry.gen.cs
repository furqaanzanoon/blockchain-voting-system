using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace VotingAPI.Services.Blockchain.Generated.ResultAggregator.ContractDefinition
{
    public partial class LeaderboardEntry : LeaderboardEntryBase { }

    public class LeaderboardEntryBase 
    {
        [Parameter("uint256", "ballotId", 1)]
        public virtual BigInteger BallotId { get; set; }
        [Parameter("address", "contractAddress", 2)]
        public virtual string ContractAddress { get; set; }
        [Parameter("string", "title", 3)]
        public virtual string Title { get; set; }
        [Parameter("uint256", "totalVotes", 4)]
        public virtual BigInteger TotalVotes { get; set; }
        [Parameter("string", "topCandidateName", 5)]
        public virtual string TopCandidateName { get; set; }
        [Parameter("uint256", "topCandidateVotes", 6)]
        public virtual BigInteger TopCandidateVotes { get; set; }
    }
}
