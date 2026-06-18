using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts;
using System.Threading;

namespace VotingAPI.Services.Blockchain.Generated.AccessControl.ContractDefinition
{


    public partial class AccessControlDeployment : AccessControlDeploymentBase
    {
        public AccessControlDeployment() : base(BYTECODE) { }
        public AccessControlDeployment(string byteCode) : base(byteCode) { }
    }

    public class AccessControlDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "0x608060405234801561000f575f5ffd5b506100407fd980155b32cf66e6af51e0972d64b9d5efe0e6f237dfaa4bdc83f990dd79e9c8806101b660201b60201c565b6100907fdf8b4c520ffe197c5343c6f5aec59570151ef9a492f2c624fd45ddde6135ec427fd980155b32cf66e6af51e0972d64b9d5efe0e6f237dfaa4bdc83f990dd79e9c86101b660201b60201c565b6100e07fd3b0a43abb6d32b41372f0dad0ad382223f68e13e854110c17ce8c88c13dbbd67fdf8b4c520ffe197c5343c6f5aec59570151ef9a492f2c624fd45ddde6135ec426101b660201b60201c565b6101307fd8994f6d76f930dc5ea8c60e38e6334a87bb8539cc3082ac6828681c33316e3d7fdf8b4c520ffe197c5343c6f5aec59570151ef9a492f2c624fd45ddde6135ec426101b660201b60201c565b6101807f15283fd96aa656c9df35ac2fcb112678a5f24f1ca97e591a97d1d16003dbfc9c7fd3b0a43abb6d32b41372f0dad0ad382223f68e13e854110c17ce8c88c13dbbd66101b660201b60201c565b6101b17fd980155b32cf66e6af51e0972d64b9d5efe0e6f237dfaa4bdc83f990dd79e9c8333361021960201b60201c565b610558565b5f5f5f8481526020019081526020015f20600101549050815f5f8581526020019081526020015f20600101819055508181847fbd79b86ffe0ab8e8776151514217cd7cacd52c909f66475c3af44e129f0b00ff60405160405180910390a4505050565b5f5f8481526020019081526020015f205f015f8373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020015f205f9054906101000a900460ff166104d65760015f5f8581526020019081526020015f205f015f8473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020015f205f6101000a81548160ff0219169083151502179055505f5f8481526020019081526020015f206002015f8154809291906102ff90610511565b919050555060015f8373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020015f2083908060018154018082558091505060019003905f5260205f20015f9091909190915055600260405180608001604052808581526020018473ffffffffffffffffffffffffffffffffffffffff1681526020014281526020018373ffffffffffffffffffffffffffffffffffffffff16815250908060018154018082558091505060019003905f5260205f2090600402015f909190919091505f820151815f01556020820151816001015f6101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff160217905550604082015181600201556060820151816003015f6101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff16021790555050508073ffffffffffffffffffffffffffffffffffffffff168273ffffffffffffffffffffffffffffffffffffffff16847f2f8788117e7eff1d82e926ec794901d17c78024a50270940304540a733656f0d60405160405180910390a45b505050565b7f4e487b71000000000000000000000000000000000000000000000000000000005f52601160045260245ffd5b5f819050919050565b5f61051b82610508565b91507fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff820361054d5761054c6104db565b5b600182019050919050565b611ef4806105655f395ff3fe608060405234801561000f575f5ffd5b5060043610610135575f3560e01c80637c7c7c3c116100b6578063aa375a8a1161007a578063aa375a8a1461034d578063c67ccea11461036b578063ca15c87314610387578063d547741f146103b7578063f568414f146103d3578063f57a044a1461040357610135565b80637c7c7c3c146102a95780638bb9c5bf146102c75780638ebf2fd6146102e357806391d1485414610301578063a6ad58ea1461033157610135565b80632f2ff15d116100fd5780632f2ff15d146101f257806346b5cb591461020e5780634d7de75e1461022a5780636e31b2761461025a5780636f39feec1461028d57610135565b80630771ba01146101395780631e4e0091146101575780631e92ef5f14610173578063248a9ca3146101a45780632a0acc6a146101d4575b5f5ffd5b610141610433565b60405161014e919061166a565b60405180910390f35b610171600480360381019061016c91906116b5565b610457565b005b61018d60048036038101906101889190611726565b6104d0565b60405161019b9291906118cb565b60405180910390f35b6101be60048036038101906101b991906118f9565b6106ee565b6040516101cb919061166a565b60405180910390f35b6101dc61070a565b6040516101e9919061166a565b60405180910390f35b61020c6004803603810190610207919061194e565b61072e565b005b610228600480360381019061022391906119ed565b61079b565b005b610244600480360381019061023f9190611a4a565b61084e565b6040516102519190611ab4565b60405180910390f35b610274600480360381019061026f9190611acd565b6108ec565b6040516102849493929190611b07565b60405180910390f35b6102a760048036038101906102a291906119ed565b610965565b005b6102b1610a18565b6040516102be919061166a565b60405180910390f35b6102e160048036038101906102dc91906118f9565b610a3c565b005b6102eb610a4a565b6040516102f8919061166a565b60405180910390f35b61031b6004803603810190610316919061194e565b610a6e565b6040516103289190611ab4565b60405180910390f35b61034b60048036038101906103469190611a4a565b610ad1565b005b610355610ca9565b604051610362919061166a565b60405180910390f35b61038560048036038101906103809190611a4a565b610ccd565b005b6103a1600480360381019061039c91906118f9565b610e36565b6040516103ae9190611b4a565b60405180910390f35b6103d160048036038101906103cc919061194e565b610e52565b005b6103ed60048036038101906103e89190611b63565b610ebf565b6040516103fa9190611c36565b60405180910390f35b61041d60048036038101906104189190611a4a565b610f52565b60405161042a9190611ab4565b60405180910390f35b7fd3b0a43abb6d32b41372f0dad0ad382223f68e13e854110c17ce8c88c13dbbd681565b7fd980155b32cf66e6af51e0972d64b9d5efe0e6f237dfaa4bdc83f990dd79e9c86104828133610a6e565b6104c1576040517f08c379a00000000000000000000000000000000000000000000000000000000081526004016104b890611cb0565b60405180910390fd5b6104cb8383610f78565b505050565b60605f600280549050905080841061053d575f67ffffffffffffffff8111156104fc576104fb611cce565b5b60405190808252806020026020018201604052801561053557816020015b610522611602565b81526020019060019003908161051a5790505b5091506106e7565b5f838561054a9190611d28565b905081811115610558578190505b84816105649190611d5b565b67ffffffffffffffff81111561057d5761057c611cce565b5b6040519080825280602002602001820160405280156105b657816020015b6105a3611602565b81526020019060019003908161059b5790505b5092505f8590505b818110156106e457600281815481106105da576105d9611d8e565b5b905f5260205f2090600402016040518060800160405290815f8201548152602001600182015f9054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200160028201548152602001600382015f9054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815250508487836106bb9190611d5b565b815181106106cc576106cb611d8e565b5b602002602001018190525080806001019150506105be565b50505b9250929050565b5f5f5f8381526020019081526020015f20600101549050919050565b7fdf8b4c520ffe197c5343c6f5aec59570151ef9a492f2c624fd45ddde6135ec4281565b5f5f8381526020019081526020015f206001015461074c8133610a6e565b61078b576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040161078290611cb0565b60405180910390fd5b610796838333610fdb565b505050565b5f5f8481526020019081526020015f20600101546107b98133610a6e565b6107f8576040517f08c379a00000000000000000000000000000000000000000000000000000000081526004016107ef90611cb0565b60405180910390fd5b5f5f90505b838390508110156108475761083a8585858481811061081f5761081e611d8e565b5b90506020020160208101906108349190611b63565b33610fdb565b80806001019150506107fd565b5050505050565b5f60035f8573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020015f205f8481526020019081526020015f205f8373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020015f205f9054906101000a900460ff1690509392505050565b600281815481106108fb575f80fd5b905f5260205f2090600402015f91509050805f015490806001015f9054906101000a900473ffffffffffffffffffffffffffffffffffffffff1690806002015490806003015f9054906101000a900473ffffffffffffffffffffffffffffffffffffffff16905084565b5f5f8481526020019081526020015f20600101546109838133610a6e565b6109c2576040517f08c379a00000000000000000000000000000000000000000000000000000000081526004016109b990611cb0565b60405180910390fd5b5f5f90505b83839050811015610a1157610a04858585848181106109e9576109e8611d8e565b5b90506020020160208101906109fe9190611b63565b3361129d565b80806001019150506109c7565b5050505050565b7fd980155b32cf66e6af51e0972d64b9d5efe0e6f237dfaa4bdc83f990dd79e9c881565b610a4781333361129d565b50565b7f15283fd96aa656c9df35ac2fcb112678a5f24f1ca97e591a97d1d16003dbfc9c81565b5f5f5f8481526020019081526020015f205f015f8373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020015f205f9054906101000a900460ff16905092915050565b7fdf8b4c520ffe197c5343c6f5aec59570151ef9a492f2c624fd45ddde6135ec42610afc8133610a6e565b610b3b576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610b3290611cb0565b60405180910390fd5b5f73ffffffffffffffffffffffffffffffffffffffff168473ffffffffffffffffffffffffffffffffffffffff1603610ba9576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610ba090611e05565b60405180910390fd5b600160035f8673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020015f205f8581526020019081526020015f205f8473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020015f205f6101000a81548160ff0219169083151502179055508173ffffffffffffffffffffffffffffffffffffffff16838573ffffffffffffffffffffffffffffffffffffffff167f5f82965f73cca0a07364954275e285c37d16b55e908b706ad6741003fad3debf60405160405180910390a450505050565b7fd8994f6d76f930dc5ea8c60e38e6334a87bb8539cc3082ac6828681c33316e3d81565b7fdf8b4c520ffe197c5343c6f5aec59570151ef9a492f2c624fd45ddde6135ec42610cf88133610a6e565b610d37576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610d2e90611cb0565b60405180910390fd5b5f60035f8673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020015f205f8581526020019081526020015f205f8473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020015f205f6101000a81548160ff0219169083151502179055508173ffffffffffffffffffffffffffffffffffffffff16838573ffffffffffffffffffffffffffffffffffffffff167f38e0deb6d66750aae1ad548eba276cd498b49ccdbffe1cbcd21a2cdc48d5778f60405160405180910390a450505050565b5f5f5f8381526020019081526020015f20600201549050919050565b5f5f8381526020019081526020015f2060010154610e708133610a6e565b610eaf576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610ea690611cb0565b60405180910390fd5b610eba83833361129d565b505050565b606060015f8373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020015f20805480602002602001604051908101604052809291908181526020018280548015610f4657602002820191905f5260205f20905b815481526020019060010190808311610f32575b50505050509050919050565b5f610f5d8383610a6e565b80610f6f5750610f6e84848461084e565b5b90509392505050565b5f5f5f8481526020019081526020015f20600101549050815f5f8581526020019081526020015f20600101819055508181847fbd79b86ffe0ab8e8776151514217cd7cacd52c909f66475c3af44e129f0b00ff60405160405180910390a4505050565b5f5f8481526020019081526020015f205f015f8373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020015f205f9054906101000a900460ff166112985760015f5f8581526020019081526020015f205f015f8473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020015f205f6101000a81548160ff0219169083151502179055505f5f8481526020019081526020015f206002015f8154809291906110c190611e23565b919050555060015f8373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020015f2083908060018154018082558091505060019003905f5260205f20015f9091909190915055600260405180608001604052808581526020018473ffffffffffffffffffffffffffffffffffffffff1681526020014281526020018373ffffffffffffffffffffffffffffffffffffffff16815250908060018154018082558091505060019003905f5260205f2090600402015f909190919091505f820151815f01556020820151816001015f6101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff160217905550604082015181600201556060820151816003015f6101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff16021790555050508073ffffffffffffffffffffffffffffffffffffffff168273ffffffffffffffffffffffffffffffffffffffff16847f2f8788117e7eff1d82e926ec794901d17c78024a50270940304540a733656f0d60405160405180910390a45b505050565b5f5f8481526020019081526020015f205f015f8373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020015f205f9054906101000a900460ff1615611503575f5f5f8581526020019081526020015f205f015f8473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020015f205f6101000a81548160ff0219169083151502179055505f5f8481526020019081526020015f206002015f81548092919061138390611e6a565b91905055506113928284611508565b600260405180608001604052808581526020018473ffffffffffffffffffffffffffffffffffffffff1681526020014281526020018373ffffffffffffffffffffffffffffffffffffffff16815250908060018154018082558091505060019003905f5260205f2090600402015f909190919091505f820151815f01556020820151816001015f6101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff160217905550604082015181600201556060820151816003015f6101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff16021790555050508073ffffffffffffffffffffffffffffffffffffffff168273ffffffffffffffffffffffffffffffffffffffff16847ff6391f5c32d9c69d2a47ea670b442974b53935d1edc7fd64eb21e047a839171b60405160405180910390a45b505050565b5f60015f8473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020015f2090505f5f90505b81805490508110156115fc578282828154811061156d5761156c611d8e565b5b905f5260205f200154036115ef57816001838054905061158d9190611d5b565b8154811061159e5761159d611d8e565b5b905f5260205f2001548282815481106115ba576115b9611d8e565b5b905f5260205f200181905550818054806115d7576115d6611e91565b5b600190038181905f5260205f20015f905590556115fc565b808060010191505061154d565b50505050565b60405180608001604052805f81526020015f73ffffffffffffffffffffffffffffffffffffffff1681526020015f81526020015f73ffffffffffffffffffffffffffffffffffffffff1681525090565b5f819050919050565b61166481611652565b82525050565b5f60208201905061167d5f83018461165b565b92915050565b5f5ffd5b5f5ffd5b61169481611652565b811461169e575f5ffd5b50565b5f813590506116af8161168b565b92915050565b5f5f604083850312156116cb576116ca611683565b5b5f6116d8858286016116a1565b92505060206116e9858286016116a1565b9150509250929050565b5f819050919050565b611705816116f3565b811461170f575f5ffd5b50565b5f81359050611720816116fc565b92915050565b5f5f6040838503121561173c5761173b611683565b5b5f61174985828601611712565b925050602061175a85828601611712565b9150509250929050565b5f81519050919050565b5f82825260208201905092915050565b5f819050602082019050919050565b61179681611652565b82525050565b5f73ffffffffffffffffffffffffffffffffffffffff82169050919050565b5f6117c58261179c565b9050919050565b6117d5816117bb565b82525050565b6117e4816116f3565b82525050565b608082015f8201516117fe5f85018261178d565b50602082015161181160208501826117cc565b50604082015161182460408501826117db565b50606082015161183760608501826117cc565b50505050565b5f61184883836117ea565b60808301905092915050565b5f602082019050919050565b5f61186a82611764565b611874818561176e565b935061187f8361177e565b805f5b838110156118af578151611896888261183d565b97506118a183611854565b925050600181019050611882565b5085935050505092915050565b6118c5816116f3565b82525050565b5f6040820190508181035f8301526118e38185611860565b90506118f260208301846118bc565b9392505050565b5f6020828403121561190e5761190d611683565b5b5f61191b848285016116a1565b91505092915050565b61192d816117bb565b8114611937575f5ffd5b50565b5f8135905061194881611924565b92915050565b5f5f6040838503121561196457611963611683565b5b5f611971858286016116a1565b92505060206119828582860161193a565b9150509250929050565b5f5ffd5b5f5ffd5b5f5ffd5b5f5f83601f8401126119ad576119ac61198c565b5b8235905067ffffffffffffffff8111156119ca576119c9611990565b5b6020830191508360208202830111156119e6576119e5611994565b5b9250929050565b5f5f5f60408486031215611a0457611a03611683565b5b5f611a11868287016116a1565b935050602084013567ffffffffffffffff811115611a3257611a31611687565b5b611a3e86828701611998565b92509250509250925092565b5f5f5f60608486031215611a6157611a60611683565b5b5f611a6e8682870161193a565b9350506020611a7f868287016116a1565b9250506040611a908682870161193a565b9150509250925092565b5f8115159050919050565b611aae81611a9a565b82525050565b5f602082019050611ac75f830184611aa5565b92915050565b5f60208284031215611ae257611ae1611683565b5b5f611aef84828501611712565b91505092915050565b611b01816117bb565b82525050565b5f608082019050611b1a5f83018761165b565b611b276020830186611af8565b611b3460408301856118bc565b611b416060830184611af8565b95945050505050565b5f602082019050611b5d5f8301846118bc565b92915050565b5f60208284031215611b7857611b77611683565b5b5f611b858482850161193a565b91505092915050565b5f81519050919050565b5f82825260208201905092915050565b5f819050602082019050919050565b5f611bc2838361178d565b60208301905092915050565b5f602082019050919050565b5f611be482611b8e565b611bee8185611b98565b9350611bf983611ba8565b805f5b83811015611c29578151611c108882611bb7565b9750611c1b83611bce565b925050600181019050611bfc565b5085935050505092915050565b5f6020820190508181035f830152611c4e8184611bda565b905092915050565b5f82825260208201905092915050565b7f416363657373436f6e74726f6c3a206d697373696e6720726f6c6500000000005f82015250565b5f611c9a601b83611c56565b9150611ca582611c66565b602082019050919050565b5f6020820190508181035f830152611cc781611c8e565b9050919050565b7f4e487b71000000000000000000000000000000000000000000000000000000005f52604160045260245ffd5b7f4e487b71000000000000000000000000000000000000000000000000000000005f52601160045260245ffd5b5f611d32826116f3565b9150611d3d836116f3565b9250828201905080821115611d5557611d54611cfb565b5b92915050565b5f611d65826116f3565b9150611d70836116f3565b9250828203905081811115611d8857611d87611cfb565b5b92915050565b7f4e487b71000000000000000000000000000000000000000000000000000000005f52603260045260245ffd5b7f5a65726f2062616c6c6f742061646472657373000000000000000000000000005f82015250565b5f611def601383611c56565b9150611dfa82611dbb565b602082019050919050565b5f6020820190508181035f830152611e1c81611de3565b9050919050565b5f611e2d826116f3565b91507fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff8203611e5f57611e5e611cfb565b5b600182019050919050565b5f611e74826116f3565b91505f8203611e8657611e85611cfb565b5b600182039050919050565b7f4e487b71000000000000000000000000000000000000000000000000000000005f52603160045260245ffdfea264697066735822122053bc8b7351bc64d77d125c44f189d15a7f24bf498a9e420250e1ff61b0f244e764736f6c634300081c0033";
        public AccessControlDeploymentBase() : base(BYTECODE) { }
        public AccessControlDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class AdminFunction : AdminFunctionBase { }

    [Function("ADMIN", "bytes32")]
    public class AdminFunctionBase : FunctionMessage
    {

    }

    public partial class AuditorFunction : AuditorFunctionBase { }

    [Function("AUDITOR", "bytes32")]
    public class AuditorFunctionBase : FunctionMessage
    {

    }

    public partial class BallotAdminFunction : BallotAdminFunctionBase { }

    [Function("BALLOT_ADMIN", "bytes32")]
    public class BallotAdminFunctionBase : FunctionMessage
    {

    }

    public partial class SuperAdminFunction : SuperAdminFunctionBase { }

    [Function("SUPER_ADMIN", "bytes32")]
    public class SuperAdminFunctionBase : FunctionMessage
    {

    }

    public partial class VoterFunction : VoterFunctionBase { }

    [Function("VOTER", "bytes32")]
    public class VoterFunctionBase : FunctionMessage
    {

    }

    public partial class BatchGrantRoleFunction : BatchGrantRoleFunctionBase { }

    [Function("batchGrantRole")]
    public class BatchGrantRoleFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
        [Parameter("address[]", "accounts", 2)]
        public virtual List<string> Accounts { get; set; }
    }

    public partial class BatchRevokeRoleFunction : BatchRevokeRoleFunctionBase { }

    [Function("batchRevokeRole")]
    public class BatchRevokeRoleFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
        [Parameter("address[]", "accounts", 2)]
        public virtual List<string> Accounts { get; set; }
    }

    public partial class GetRoleAdminFunction : GetRoleAdminFunctionBase { }

    [Function("getRoleAdmin", "bytes32")]
    public class GetRoleAdminFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
    }

    public partial class GetRoleHistoryFunction : GetRoleHistoryFunctionBase { }

    [Function("getRoleHistory", typeof(GetRoleHistoryOutputDTO))]
    public class GetRoleHistoryFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "_offset", 1)]
        public virtual BigInteger Offset { get; set; }
        [Parameter("uint256", "_limit", 2)]
        public virtual BigInteger Limit { get; set; }
    }

    public partial class GetRoleMemberCountFunction : GetRoleMemberCountFunctionBase { }

    [Function("getRoleMemberCount", "uint256")]
    public class GetRoleMemberCountFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
    }

    public partial class GetRolesOfFunction : GetRolesOfFunctionBase { }

    [Function("getRolesOf", "bytes32[]")]
    public class GetRolesOfFunctionBase : FunctionMessage
    {
        [Parameter("address", "account", 1)]
        public virtual string Account { get; set; }
    }

    public partial class GrantBallotRoleFunction : GrantBallotRoleFunctionBase { }

    [Function("grantBallotRole")]
    public class GrantBallotRoleFunctionBase : FunctionMessage
    {
        [Parameter("address", "ballot", 1)]
        public virtual string Ballot { get; set; }
        [Parameter("bytes32", "role", 2)]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "account", 3)]
        public virtual string Account { get; set; }
    }

    public partial class GrantRoleFunction : GrantRoleFunctionBase { }

    [Function("grantRole")]
    public class GrantRoleFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "account", 2)]
        public virtual string Account { get; set; }
    }

    public partial class HasBallotRoleFunction : HasBallotRoleFunctionBase { }

    [Function("hasBallotRole", "bool")]
    public class HasBallotRoleFunctionBase : FunctionMessage
    {
        [Parameter("address", "ballot", 1)]
        public virtual string Ballot { get; set; }
        [Parameter("bytes32", "role", 2)]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "account", 3)]
        public virtual string Account { get; set; }
    }

    public partial class HasRoleFunction : HasRoleFunctionBase { }

    [Function("hasRole", "bool")]
    public class HasRoleFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "account", 2)]
        public virtual string Account { get; set; }
    }

    public partial class IsAuthorizedFunction : IsAuthorizedFunctionBase { }

    [Function("isAuthorized", "bool")]
    public class IsAuthorizedFunctionBase : FunctionMessage
    {
        [Parameter("address", "ballot", 1)]
        public virtual string Ballot { get; set; }
        [Parameter("bytes32", "role", 2)]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "account", 3)]
        public virtual string Account { get; set; }
    }

    public partial class RenounceRoleFunction : RenounceRoleFunctionBase { }

    [Function("renounceRole")]
    public class RenounceRoleFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
    }

    public partial class RevokeBallotRoleFunction : RevokeBallotRoleFunctionBase { }

    [Function("revokeBallotRole")]
    public class RevokeBallotRoleFunctionBase : FunctionMessage
    {
        [Parameter("address", "ballot", 1)]
        public virtual string Ballot { get; set; }
        [Parameter("bytes32", "role", 2)]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "account", 3)]
        public virtual string Account { get; set; }
    }

    public partial class RevokeRoleFunction : RevokeRoleFunctionBase { }

    [Function("revokeRole")]
    public class RevokeRoleFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "account", 2)]
        public virtual string Account { get; set; }
    }

    public partial class RoleHistoryFunction : RoleHistoryFunctionBase { }

    [Function("roleHistory", typeof(RoleHistoryOutputDTO))]
    public class RoleHistoryFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class SetRoleAdminFunction : SetRoleAdminFunctionBase { }

    [Function("setRoleAdmin")]
    public class SetRoleAdminFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "role", 1)]
        public virtual byte[] Role { get; set; }
        [Parameter("bytes32", "adminRole", 2)]
        public virtual byte[] AdminRole { get; set; }
    }

    public partial class BallotRoleGrantedEventDTO : BallotRoleGrantedEventDTOBase { }

    [Event("BallotRoleGranted")]
    public class BallotRoleGrantedEventDTOBase : IEventDTO
    {
        [Parameter("address", "ballot", 1, true )]
        public virtual string Ballot { get; set; }
        [Parameter("bytes32", "role", 2, true )]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "account", 3, true )]
        public virtual string Account { get; set; }
    }

    public partial class BallotRoleRevokedEventDTO : BallotRoleRevokedEventDTOBase { }

    [Event("BallotRoleRevoked")]
    public class BallotRoleRevokedEventDTOBase : IEventDTO
    {
        [Parameter("address", "ballot", 1, true )]
        public virtual string Ballot { get; set; }
        [Parameter("bytes32", "role", 2, true )]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "account", 3, true )]
        public virtual string Account { get; set; }
    }

    public partial class RoleAdminChangedEventDTO : RoleAdminChangedEventDTOBase { }

    [Event("RoleAdminChanged")]
    public class RoleAdminChangedEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "role", 1, true )]
        public virtual byte[] Role { get; set; }
        [Parameter("bytes32", "previousAdmin", 2, true )]
        public virtual byte[] PreviousAdmin { get; set; }
        [Parameter("bytes32", "newAdmin", 3, true )]
        public virtual byte[] NewAdmin { get; set; }
    }

    public partial class RoleGrantedEventDTO : RoleGrantedEventDTOBase { }

    [Event("RoleGranted")]
    public class RoleGrantedEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "role", 1, true )]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "account", 2, true )]
        public virtual string Account { get; set; }
        [Parameter("address", "sender", 3, true )]
        public virtual string Sender { get; set; }
    }

    public partial class RoleRevokedEventDTO : RoleRevokedEventDTOBase { }

    [Event("RoleRevoked")]
    public class RoleRevokedEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "role", 1, true )]
        public virtual byte[] Role { get; set; }
        [Parameter("address", "account", 2, true )]
        public virtual string Account { get; set; }
        [Parameter("address", "sender", 3, true )]
        public virtual string Sender { get; set; }
    }

    public partial class AdminOutputDTO : AdminOutputDTOBase { }

    [FunctionOutput]
    public class AdminOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class AuditorOutputDTO : AuditorOutputDTOBase { }

    [FunctionOutput]
    public class AuditorOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class BallotAdminOutputDTO : BallotAdminOutputDTOBase { }

    [FunctionOutput]
    public class BallotAdminOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class SuperAdminOutputDTO : SuperAdminOutputDTOBase { }

    [FunctionOutput]
    public class SuperAdminOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class VoterOutputDTO : VoterOutputDTOBase { }

    [FunctionOutput]
    public class VoterOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }





    public partial class GetRoleAdminOutputDTO : GetRoleAdminOutputDTOBase { }

    [FunctionOutput]
    public class GetRoleAdminOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class GetRoleHistoryOutputDTO : GetRoleHistoryOutputDTOBase { }

    [FunctionOutput]
    public class GetRoleHistoryOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("tuple[]", "page", 1)]
        public virtual List<RoleInfo> Page { get; set; }
        [Parameter("uint256", "total", 2)]
        public virtual BigInteger Total { get; set; }
    }

    public partial class GetRoleMemberCountOutputDTO : GetRoleMemberCountOutputDTOBase { }

    [FunctionOutput]
    public class GetRoleMemberCountOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class GetRolesOfOutputDTO : GetRolesOfOutputDTOBase { }

    [FunctionOutput]
    public class GetRolesOfOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes32[]", "", 1)]
        public virtual List<byte[]> ReturnValue1 { get; set; }
    }





    public partial class HasBallotRoleOutputDTO : HasBallotRoleOutputDTOBase { }

    [FunctionOutput]
    public class HasBallotRoleOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }

    public partial class HasRoleOutputDTO : HasRoleOutputDTOBase { }

    [FunctionOutput]
    public class HasRoleOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }

    public partial class IsAuthorizedOutputDTO : IsAuthorizedOutputDTOBase { }

    [FunctionOutput]
    public class IsAuthorizedOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }







    public partial class RoleHistoryOutputDTO : RoleHistoryOutputDTOBase { }

    [FunctionOutput]
    public class RoleHistoryOutputDTOBase : IFunctionOutputDTO 
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
