using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace VotingAPI.Services.Blockchain.Generated.VoterRegistry.ContractDefinition
{
    public partial class Voter : VoterBase { }

    public class VoterBase 
    {
        [Parameter("address", "wallet", 1)]
        public virtual string Wallet { get; set; }
        [Parameter("string", "name", 2)]
        public virtual string Name { get; set; }
        [Parameter("uint8", "status", 3)]
        public virtual byte Status { get; set; }
        [Parameter("uint256", "registeredAt", 4)]
        public virtual BigInteger RegisteredAt { get; set; }
        [Parameter("uint256", "approvedAt", 5)]
        public virtual BigInteger ApprovedAt { get; set; }
    }
}
