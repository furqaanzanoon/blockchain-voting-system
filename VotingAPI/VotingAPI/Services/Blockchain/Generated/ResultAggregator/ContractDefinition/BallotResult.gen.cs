using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace VotingAPI.Services.Blockchain.Generated.ResultAggregator.ContractDefinition
{
    public partial class BallotResult : BallotResultBase { }

    public class BallotResultBase 
    {
        [Parameter("uint256", "ballotId", 1)]
        public virtual BigInteger BallotId { get; set; }
        [Parameter("address", "contractAddress", 2)]
        public virtual string ContractAddress { get; set; }
        [Parameter("address", "owner", 3)]
        public virtual string Owner { get; set; }
        [Parameter("string", "title", 4)]
        public virtual string Title { get; set; }
        [Parameter("bool", "votingOpen", 5)]
        public virtual bool VotingOpen { get; set; }
        [Parameter("uint256", "totalVotes", 6)]
        public virtual BigInteger TotalVotes { get; set; }
        [Parameter("tuple[]", "candidates", 7)]
        public virtual List<CandidateResult> Candidates { get; set; }
        [Parameter("uint256[]", "winnerIds", 8)]
        public virtual List<BigInteger> WinnerIds { get; set; }
    }
}
