using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace VotingAPI.Services.Blockchain.Generated.BallotFactory.ContractDefinition
{
    public partial class BallotInfo : BallotInfoBase { }

    public class BallotInfoBase 
    {
        [Parameter("uint256", "ballotId", 1)]
        public virtual BigInteger BallotId { get; set; }
        [Parameter("address", "contractAddress", 2)]
        public virtual string ContractAddress { get; set; }
        [Parameter("address", "owner", 3)]
        public virtual string Owner { get; set; }
        [Parameter("string", "title", 4)]
        public virtual string Title { get; set; }
    }
}
