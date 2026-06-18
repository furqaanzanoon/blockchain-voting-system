using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace VotingAPI.Services.Blockchain.Generated.ResultAggregator.ContractDefinition
{
    public partial class CandidateResult : CandidateResultBase { }

    public class CandidateResultBase 
    {
        [Parameter("uint256", "candidateId", 1)]
        public virtual BigInteger CandidateId { get; set; }
        [Parameter("string", "name", 2)]
        public virtual string Name { get; set; }
        [Parameter("uint256", "voteCount", 3)]
        public virtual BigInteger VoteCount { get; set; }
        [Parameter("uint256", "percentage", 4)]
        public virtual BigInteger Percentage { get; set; }
    }
}
