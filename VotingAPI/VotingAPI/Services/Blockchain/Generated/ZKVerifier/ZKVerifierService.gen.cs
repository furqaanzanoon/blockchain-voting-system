using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using VotingAPI.Services.Blockchain.Generated.ZKVerifier.ContractDefinition;

namespace VotingAPI.Services.Blockchain.Generated.ZKVerifier
{
    public partial class ZKVerifierService: ZKVerifierServiceBase
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.IWeb3 web3, ZKVerifierDeployment zKVerifierDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<ZKVerifierDeployment>().SendRequestAndWaitForReceiptAsync(zKVerifierDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.IWeb3 web3, ZKVerifierDeployment zKVerifierDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<ZKVerifierDeployment>().SendRequestAsync(zKVerifierDeployment);
        }

        public static async Task<ZKVerifierService> DeployContractAndGetServiceAsync(Nethereum.Web3.IWeb3 web3, ZKVerifierDeployment zKVerifierDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, zKVerifierDeployment, cancellationTokenSource);
            return new ZKVerifierService(web3, receipt.ContractAddress);
        }

        public ZKVerifierService(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

    }


    public partial class ZKVerifierServiceBase: ContractWeb3ServiceBase
    {

        public ZKVerifierServiceBase(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

        public Task<BigInteger> BallotRootsQueryAsync(BallotRootsFunction ballotRootsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BallotRootsFunction, BigInteger>(ballotRootsFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> BallotRootsQueryAsync(BigInteger returnValue1, BlockParameter blockParameter = null)
        {
            var ballotRootsFunction = new BallotRootsFunction();
                ballotRootsFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<BallotRootsFunction, BigInteger>(ballotRootsFunction, blockParameter);
        }

        public Task<List<BigInteger>> GetVoteCommitmentsQueryAsync(GetVoteCommitmentsFunction getVoteCommitmentsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetVoteCommitmentsFunction, List<BigInteger>>(getVoteCommitmentsFunction, blockParameter);
        }

        
        public virtual Task<List<BigInteger>> GetVoteCommitmentsQueryAsync(BigInteger ballotId, BlockParameter blockParameter = null)
        {
            var getVoteCommitmentsFunction = new GetVoteCommitmentsFunction();
                getVoteCommitmentsFunction.BallotId = ballotId;
            
            return ContractHandler.QueryAsync<GetVoteCommitmentsFunction, List<BigInteger>>(getVoteCommitmentsFunction, blockParameter);
        }

        public Task<BigInteger> GetVoteCountQueryAsync(GetVoteCountFunction getVoteCountFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetVoteCountFunction, BigInteger>(getVoteCountFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> GetVoteCountQueryAsync(BigInteger ballotId, BlockParameter blockParameter = null)
        {
            var getVoteCountFunction = new GetVoteCountFunction();
                getVoteCountFunction.BallotId = ballotId;
            
            return ContractHandler.QueryAsync<GetVoteCountFunction, BigInteger>(getVoteCountFunction, blockParameter);
        }

        public Task<bool> IsNullifierSpentQueryAsync(IsNullifierSpentFunction isNullifierSpentFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsNullifierSpentFunction, bool>(isNullifierSpentFunction, blockParameter);
        }

        
        public virtual Task<bool> IsNullifierSpentQueryAsync(BigInteger nullifier, BlockParameter blockParameter = null)
        {
            var isNullifierSpentFunction = new IsNullifierSpentFunction();
                isNullifierSpentFunction.Nullifier = nullifier;
            
            return ContractHandler.QueryAsync<IsNullifierSpentFunction, bool>(isNullifierSpentFunction, blockParameter);
        }

        public Task<bool> NullifierSeenQueryAsync(NullifierSeenFunction nullifierSeenFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NullifierSeenFunction, bool>(nullifierSeenFunction, blockParameter);
        }

        
        public virtual Task<bool> NullifierSeenQueryAsync(BigInteger returnValue1, BlockParameter blockParameter = null)
        {
            var nullifierSeenFunction = new NullifierSeenFunction();
                nullifierSeenFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<NullifierSeenFunction, bool>(nullifierSeenFunction, blockParameter);
        }

        public Task<BigInteger> NullifierUsedQueryAsync(NullifierUsedFunction nullifierUsedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NullifierUsedFunction, BigInteger>(nullifierUsedFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> NullifierUsedQueryAsync(BigInteger returnValue1, BlockParameter blockParameter = null)
        {
            var nullifierUsedFunction = new NullifierUsedFunction();
                nullifierUsedFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<NullifierUsedFunction, BigInteger>(nullifierUsedFunction, blockParameter);
        }

        public Task<string> OwnerQueryAsync(OwnerFunction ownerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(ownerFunction, blockParameter);
        }

        
        public virtual Task<string> OwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(null, blockParameter);
        }

        public virtual Task<string> SetBallotRootRequestAsync(SetBallotRootFunction setBallotRootFunction)
        {
             return ContractHandler.SendRequestAsync(setBallotRootFunction);
        }

        public virtual Task<TransactionReceipt> SetBallotRootRequestAndWaitForReceiptAsync(SetBallotRootFunction setBallotRootFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setBallotRootFunction, cancellationToken);
        }

        public virtual Task<string> SetBallotRootRequestAsync(BigInteger ballotId, BigInteger merkleRoot)
        {
            var setBallotRootFunction = new SetBallotRootFunction();
                setBallotRootFunction.BallotId = ballotId;
                setBallotRootFunction.MerkleRoot = merkleRoot;
            
             return ContractHandler.SendRequestAsync(setBallotRootFunction);
        }

        public virtual Task<TransactionReceipt> SetBallotRootRequestAndWaitForReceiptAsync(BigInteger ballotId, BigInteger merkleRoot, CancellationTokenSource cancellationToken = null)
        {
            var setBallotRootFunction = new SetBallotRootFunction();
                setBallotRootFunction.BallotId = ballotId;
                setBallotRootFunction.MerkleRoot = merkleRoot;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setBallotRootFunction, cancellationToken);
        }

        public virtual Task<string> SetVerifyingKeyRequestAsync(SetVerifyingKeyFunction setVerifyingKeyFunction)
        {
             return ContractHandler.SendRequestAsync(setVerifyingKeyFunction);
        }

        public virtual Task<TransactionReceipt> SetVerifyingKeyRequestAndWaitForReceiptAsync(SetVerifyingKeyFunction setVerifyingKeyFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setVerifyingKeyFunction, cancellationToken);
        }

        public virtual Task<string> SetVerifyingKeyRequestAsync(List<BigInteger> alpha1, List<List<BigInteger>> beta2, List<List<BigInteger>> gamma2, List<List<BigInteger>> delta2, List<List<BigInteger>> ic)
        {
            var setVerifyingKeyFunction = new SetVerifyingKeyFunction();
                setVerifyingKeyFunction.Alpha1 = alpha1;
                setVerifyingKeyFunction.Beta2 = beta2;
                setVerifyingKeyFunction.Gamma2 = gamma2;
                setVerifyingKeyFunction.Delta2 = delta2;
                setVerifyingKeyFunction.Ic = ic;
            
             return ContractHandler.SendRequestAsync(setVerifyingKeyFunction);
        }

        public virtual Task<TransactionReceipt> SetVerifyingKeyRequestAndWaitForReceiptAsync(List<BigInteger> alpha1, List<List<BigInteger>> beta2, List<List<BigInteger>> gamma2, List<List<BigInteger>> delta2, List<List<BigInteger>> ic, CancellationTokenSource cancellationToken = null)
        {
            var setVerifyingKeyFunction = new SetVerifyingKeyFunction();
                setVerifyingKeyFunction.Alpha1 = alpha1;
                setVerifyingKeyFunction.Beta2 = beta2;
                setVerifyingKeyFunction.Gamma2 = gamma2;
                setVerifyingKeyFunction.Delta2 = delta2;
                setVerifyingKeyFunction.Ic = ic;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setVerifyingKeyFunction, cancellationToken);
        }

        public virtual Task<string> VerifyAndVoteRequestAsync(VerifyAndVoteFunction verifyAndVoteFunction)
        {
             return ContractHandler.SendRequestAsync(verifyAndVoteFunction);
        }

        public virtual Task<TransactionReceipt> VerifyAndVoteRequestAndWaitForReceiptAsync(VerifyAndVoteFunction verifyAndVoteFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(verifyAndVoteFunction, cancellationToken);
        }

        public virtual Task<string> VerifyAndVoteRequestAsync(Proof proof, PublicSignals signals)
        {
            var verifyAndVoteFunction = new VerifyAndVoteFunction();
                verifyAndVoteFunction.Proof = proof;
                verifyAndVoteFunction.Signals = signals;
            
             return ContractHandler.SendRequestAsync(verifyAndVoteFunction);
        }

        public virtual Task<TransactionReceipt> VerifyAndVoteRequestAndWaitForReceiptAsync(Proof proof, PublicSignals signals, CancellationTokenSource cancellationToken = null)
        {
            var verifyAndVoteFunction = new VerifyAndVoteFunction();
                verifyAndVoteFunction.Proof = proof;
                verifyAndVoteFunction.Signals = signals;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(verifyAndVoteFunction, cancellationToken);
        }

        public Task<bool> VkSetQueryAsync(VkSetFunction vkSetFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VkSetFunction, bool>(vkSetFunction, blockParameter);
        }

        
        public virtual Task<bool> VkSetQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VkSetFunction, bool>(null, blockParameter);
        }

        public Task<BigInteger> VoteCommitmentsQueryAsync(VoteCommitmentsFunction voteCommitmentsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VoteCommitmentsFunction, BigInteger>(voteCommitmentsFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> VoteCommitmentsQueryAsync(BigInteger returnValue1, BigInteger returnValue2, BlockParameter blockParameter = null)
        {
            var voteCommitmentsFunction = new VoteCommitmentsFunction();
                voteCommitmentsFunction.ReturnValue1 = returnValue1;
                voteCommitmentsFunction.ReturnValue2 = returnValue2;
            
            return ContractHandler.QueryAsync<VoteCommitmentsFunction, BigInteger>(voteCommitmentsFunction, blockParameter);
        }

        public override List<Type> GetAllFunctionTypes()
        {
            return new List<Type>
            {
                typeof(BallotRootsFunction),
                typeof(GetVoteCommitmentsFunction),
                typeof(GetVoteCountFunction),
                typeof(IsNullifierSpentFunction),
                typeof(NullifierSeenFunction),
                typeof(NullifierUsedFunction),
                typeof(OwnerFunction),
                typeof(SetBallotRootFunction),
                typeof(SetVerifyingKeyFunction),
                typeof(VerifyAndVoteFunction),
                typeof(VkSetFunction),
                typeof(VoteCommitmentsFunction)
            };
        }

        public override List<Type> GetAllEventTypes()
        {
            return new List<Type>
            {
                typeof(BallotRootSetEventDTO),
                typeof(NullifierSpentEventDTO),
                typeof(VerifyingKeySetEventDTO),
                typeof(VoteVerifiedEventDTO)
            };
        }

        public override List<Type> GetAllErrorTypes()
        {
            return new List<Type>
            {

            };
        }
    }
}
