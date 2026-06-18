using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace VotingAPI.Services.Blockchain.Generated.ZKVerifier.ContractDefinition
{
    public partial class Proof : ProofBase { }

    public class ProofBase 
    {
        [Parameter("tuple", "a", 1)]
        public virtual G1Point A { get; set; }
        [Parameter("tuple", "b", 2)]
        public virtual G2Point B { get; set; }
        [Parameter("tuple", "c", 3)]
        public virtual G1Point C { get; set; }
    }
}
