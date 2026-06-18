using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace VotingAPI.Services.Blockchain.Generated.ZKVerifier.ContractDefinition
{
    public partial class G2Point : G2PointBase { }

    public class G2PointBase 
    {
        [Parameter("uint256[2]", "x", 1)]
        public virtual List<BigInteger> X { get; set; }
        [Parameter("uint256[2]", "y", 2)]
        public virtual List<BigInteger> Y { get; set; }
    }
}
