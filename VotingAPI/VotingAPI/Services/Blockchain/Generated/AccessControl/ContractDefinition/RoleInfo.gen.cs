using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace VotingAPI.Services.Blockchain.Generated.AccessControl.ContractDefinition
{
    public partial class RoleInfo : RoleInfoBase { }

    public class RoleInfoBase 
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "account", 2)]
        public virtual string Account { get; set; }
        [Parameter("uint256", "grantedAt", 3)]
        public virtual BigInteger GrantedAt { get; set; }
        [Parameter("address", "grantedBy", 4)]
        public virtual string GrantedBy { get; set; }
    }
}
