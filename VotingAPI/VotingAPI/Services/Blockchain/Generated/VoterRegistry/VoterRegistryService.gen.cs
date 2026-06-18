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
using VotingAPI.Services.Blockchain.Generated.VoterRegistry.ContractDefinition;

namespace VotingAPI.Services.Blockchain.Generated.VoterRegistry
{
    public partial class VoterRegistryService: VoterRegistryServiceBase
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.IWeb3 web3, VoterRegistryDeployment voterRegistryDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<VoterRegistryDeployment>().SendRequestAndWaitForReceiptAsync(voterRegistryDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.IWeb3 web3, VoterRegistryDeployment voterRegistryDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<VoterRegistryDeployment>().SendRequestAsync(voterRegistryDeployment);
        }

        public static async Task<VoterRegistryService> DeployContractAndGetServiceAsync(Nethereum.Web3.IWeb3 web3, VoterRegistryDeployment voterRegistryDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, voterRegistryDeployment, cancellationTokenSource);
            return new VoterRegistryService(web3, receipt.ContractAddress);
        }

        public VoterRegistryService(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

    }


    public partial class VoterRegistryServiceBase: ContractWeb3ServiceBase
    {

        public VoterRegistryServiceBase(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

        public Task<string> AclQueryAsync(AclFunction aclFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AclFunction, string>(aclFunction, blockParameter);
        }

        
        public virtual Task<string> AclQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AclFunction, string>(null, blockParameter);
        }

        public virtual Task<string> ApproveVoterRequestAsync(ApproveVoterFunction approveVoterFunction)
        {
             return ContractHandler.SendRequestAsync(approveVoterFunction);
        }

        public virtual Task<TransactionReceipt> ApproveVoterRequestAndWaitForReceiptAsync(ApproveVoterFunction approveVoterFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveVoterFunction, cancellationToken);
        }

        public virtual Task<string> ApproveVoterRequestAsync(string voter)
        {
            var approveVoterFunction = new ApproveVoterFunction();
                approveVoterFunction.Voter = voter;
            
             return ContractHandler.SendRequestAsync(approveVoterFunction);
        }

        public virtual Task<TransactionReceipt> ApproveVoterRequestAndWaitForReceiptAsync(string voter, CancellationTokenSource cancellationToken = null)
        {
            var approveVoterFunction = new ApproveVoterFunction();
                approveVoterFunction.Voter = voter;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveVoterFunction, cancellationToken);
        }

        public virtual Task<string> BatchApproveRequestAsync(BatchApproveFunction batchApproveFunction)
        {
             return ContractHandler.SendRequestAsync(batchApproveFunction);
        }

        public virtual Task<TransactionReceipt> BatchApproveRequestAndWaitForReceiptAsync(BatchApproveFunction batchApproveFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchApproveFunction, cancellationToken);
        }

        public virtual Task<string> BatchApproveRequestAsync(List<string> voters)
        {
            var batchApproveFunction = new BatchApproveFunction();
                batchApproveFunction.Voters = voters;
            
             return ContractHandler.SendRequestAsync(batchApproveFunction);
        }

        public virtual Task<TransactionReceipt> BatchApproveRequestAndWaitForReceiptAsync(List<string> voters, CancellationTokenSource cancellationToken = null)
        {
            var batchApproveFunction = new BatchApproveFunction();
                batchApproveFunction.Voters = voters;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchApproveFunction, cancellationToken);
        }

        public virtual Task<GetVoterOutputDTO> GetVoterQueryAsync(GetVoterFunction getVoterFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetVoterFunction, GetVoterOutputDTO>(getVoterFunction, blockParameter);
        }

        public virtual Task<GetVoterOutputDTO> GetVoterQueryAsync(string voter, BlockParameter blockParameter = null)
        {
            var getVoterFunction = new GetVoterFunction();
                getVoterFunction.Voter = voter;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetVoterFunction, GetVoterOutputDTO>(getVoterFunction, blockParameter);
        }

        public Task<BigInteger> GetVoterCountQueryAsync(GetVoterCountFunction getVoterCountFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetVoterCountFunction, BigInteger>(getVoterCountFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> GetVoterCountQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetVoterCountFunction, BigInteger>(null, blockParameter);
        }

        public virtual Task<GetVotersPaginatedOutputDTO> GetVotersPaginatedQueryAsync(GetVotersPaginatedFunction getVotersPaginatedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetVotersPaginatedFunction, GetVotersPaginatedOutputDTO>(getVotersPaginatedFunction, blockParameter);
        }

        public virtual Task<GetVotersPaginatedOutputDTO> GetVotersPaginatedQueryAsync(BigInteger offset, BigInteger limit, BlockParameter blockParameter = null)
        {
            var getVotersPaginatedFunction = new GetVotersPaginatedFunction();
                getVotersPaginatedFunction.Offset = offset;
                getVotersPaginatedFunction.Limit = limit;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetVotersPaginatedFunction, GetVotersPaginatedOutputDTO>(getVotersPaginatedFunction, blockParameter);
        }

        public Task<bool> IsApprovedQueryAsync(IsApprovedFunction isApprovedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsApprovedFunction, bool>(isApprovedFunction, blockParameter);
        }

        
        public virtual Task<bool> IsApprovedQueryAsync(string voter, BlockParameter blockParameter = null)
        {
            var isApprovedFunction = new IsApprovedFunction();
                isApprovedFunction.Voter = voter;
            
            return ContractHandler.QueryAsync<IsApprovedFunction, bool>(isApprovedFunction, blockParameter);
        }

        public Task<bool> IsEligibleQueryAsync(IsEligibleFunction isEligibleFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsEligibleFunction, bool>(isEligibleFunction, blockParameter);
        }

        
        public virtual Task<bool> IsEligibleQueryAsync(string ballot, string voter, BlockParameter blockParameter = null)
        {
            var isEligibleFunction = new IsEligibleFunction();
                isEligibleFunction.Ballot = ballot;
                isEligibleFunction.Voter = voter;
            
            return ContractHandler.QueryAsync<IsEligibleFunction, bool>(isEligibleFunction, blockParameter);
        }

        public virtual Task<string> RegisterRequestAsync(RegisterFunction registerFunction)
        {
             return ContractHandler.SendRequestAsync(registerFunction);
        }

        public virtual Task<TransactionReceipt> RegisterRequestAndWaitForReceiptAsync(RegisterFunction registerFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(registerFunction, cancellationToken);
        }

        public virtual Task<string> RegisterRequestAsync(string name)
        {
            var registerFunction = new RegisterFunction();
                registerFunction.Name = name;
            
             return ContractHandler.SendRequestAsync(registerFunction);
        }

        public virtual Task<TransactionReceipt> RegisterRequestAndWaitForReceiptAsync(string name, CancellationTokenSource cancellationToken = null)
        {
            var registerFunction = new RegisterFunction();
                registerFunction.Name = name;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(registerFunction, cancellationToken);
        }

        public virtual Task<string> RevokeVoterRequestAsync(RevokeVoterFunction revokeVoterFunction)
        {
             return ContractHandler.SendRequestAsync(revokeVoterFunction);
        }

        public virtual Task<TransactionReceipt> RevokeVoterRequestAndWaitForReceiptAsync(RevokeVoterFunction revokeVoterFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeVoterFunction, cancellationToken);
        }

        public virtual Task<string> RevokeVoterRequestAsync(string voter)
        {
            var revokeVoterFunction = new RevokeVoterFunction();
                revokeVoterFunction.Voter = voter;
            
             return ContractHandler.SendRequestAsync(revokeVoterFunction);
        }

        public virtual Task<TransactionReceipt> RevokeVoterRequestAndWaitForReceiptAsync(string voter, CancellationTokenSource cancellationToken = null)
        {
            var revokeVoterFunction = new RevokeVoterFunction();
                revokeVoterFunction.Voter = voter;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeVoterFunction, cancellationToken);
        }

        public virtual Task<string> SetEligibilityRequestAsync(SetEligibilityFunction setEligibilityFunction)
        {
             return ContractHandler.SendRequestAsync(setEligibilityFunction);
        }

        public virtual Task<TransactionReceipt> SetEligibilityRequestAndWaitForReceiptAsync(SetEligibilityFunction setEligibilityFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setEligibilityFunction, cancellationToken);
        }

        public virtual Task<string> SetEligibilityRequestAsync(string ballot, string voter, bool eligible)
        {
            var setEligibilityFunction = new SetEligibilityFunction();
                setEligibilityFunction.Ballot = ballot;
                setEligibilityFunction.Voter = voter;
                setEligibilityFunction.Eligible = eligible;
            
             return ContractHandler.SendRequestAsync(setEligibilityFunction);
        }

        public virtual Task<TransactionReceipt> SetEligibilityRequestAndWaitForReceiptAsync(string ballot, string voter, bool eligible, CancellationTokenSource cancellationToken = null)
        {
            var setEligibilityFunction = new SetEligibilityFunction();
                setEligibilityFunction.Ballot = ballot;
                setEligibilityFunction.Voter = voter;
                setEligibilityFunction.Eligible = eligible;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setEligibilityFunction, cancellationToken);
        }

        public override List<Type> GetAllFunctionTypes()
        {
            return new List<Type>
            {
                typeof(AclFunction),
                typeof(ApproveVoterFunction),
                typeof(BatchApproveFunction),
                typeof(GetVoterFunction),
                typeof(GetVoterCountFunction),
                typeof(GetVotersPaginatedFunction),
                typeof(IsApprovedFunction),
                typeof(IsEligibleFunction),
                typeof(RegisterFunction),
                typeof(RevokeVoterFunction),
                typeof(SetEligibilityFunction)
            };
        }

        public override List<Type> GetAllEventTypes()
        {
            return new List<Type>
            {
                typeof(EligibilitySetEventDTO),
                typeof(VoterApprovedEventDTO),
                typeof(VoterRegisteredEventDTO),
                typeof(VoterRevokedEventDTO)
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
