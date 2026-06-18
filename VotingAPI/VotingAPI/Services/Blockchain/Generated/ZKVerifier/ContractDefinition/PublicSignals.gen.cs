using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace VotingAPI.Services.Blockchain.Generated.ZKVerifier.ContractDefinition
{
    public partial class PublicSignals : PublicSignalsBase { }

    public class PublicSignalsBase 
    {
        [Parameter("uint256", "merkleRoot", 1)]
        public virtual BigInteger MerkleRoot { get; set; }
        [Parameter("uint256", "nullifierHash", 2)]
        public virtual BigInteger NullifierHash { get; set; }
        [Parameter("uint256", "ballotId", 3)]
        public virtual BigInteger BallotId { get; set; }
        [Parameter("uint256", "voteCommitment", 4)]
        public virtual BigInteger VoteCommitment { get; set; }
    }
}
