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
using VotingAPI.Services.Blockchain.Generated.Voting.ContractDefinition;

namespace VotingAPI.Services.Blockchain.Generated.Voting
{
    public partial class VotingService: VotingServiceBase
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.IWeb3 web3, VotingDeployment votingDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<VotingDeployment>().SendRequestAndWaitForReceiptAsync(votingDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.IWeb3 web3, VotingDeployment votingDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<VotingDeployment>().SendRequestAsync(votingDeployment);
        }

        public static async Task<VotingService> DeployContractAndGetServiceAsync(Nethereum.Web3.IWeb3 web3, VotingDeployment votingDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, votingDeployment, cancellationTokenSource);
            return new VotingService(web3, receipt.ContractAddress);
        }

        public VotingService(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

    }


    public partial class VotingServiceBase: ContractWeb3ServiceBase
    {

        public VotingServiceBase(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

        public virtual Task<string> AddCandidateRequestAsync(AddCandidateFunction addCandidateFunction)
        {
             return ContractHandler.SendRequestAsync(addCandidateFunction);
        }

        public virtual Task<TransactionReceipt> AddCandidateRequestAndWaitForReceiptAsync(AddCandidateFunction addCandidateFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addCandidateFunction, cancellationToken);
        }

        public virtual Task<string> AddCandidateRequestAsync(string name)
        {
            var addCandidateFunction = new AddCandidateFunction();
                addCandidateFunction.Name = name;
            
             return ContractHandler.SendRequestAsync(addCandidateFunction);
        }

        public virtual Task<TransactionReceipt> AddCandidateRequestAndWaitForReceiptAsync(string name, CancellationTokenSource cancellationToken = null)
        {
            var addCandidateFunction = new AddCandidateFunction();
                addCandidateFunction.Name = name;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(addCandidateFunction, cancellationToken);
        }

        public virtual Task<CandidatesOutputDTO> CandidatesQueryAsync(CandidatesFunction candidatesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<CandidatesFunction, CandidatesOutputDTO>(candidatesFunction, blockParameter);
        }

        public virtual Task<CandidatesOutputDTO> CandidatesQueryAsync(BigInteger returnValue1, BlockParameter blockParameter = null)
        {
            var candidatesFunction = new CandidatesFunction();
                candidatesFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryDeserializingToObjectAsync<CandidatesFunction, CandidatesOutputDTO>(candidatesFunction, blockParameter);
        }

        public virtual Task<string> EndVotingRequestAsync(EndVotingFunction endVotingFunction)
        {
             return ContractHandler.SendRequestAsync(endVotingFunction);
        }

        public virtual Task<string> EndVotingRequestAsync()
        {
             return ContractHandler.SendRequestAsync<EndVotingFunction>();
        }

        public virtual Task<TransactionReceipt> EndVotingRequestAndWaitForReceiptAsync(EndVotingFunction endVotingFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(endVotingFunction, cancellationToken);
        }

        public virtual Task<TransactionReceipt> EndVotingRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync<EndVotingFunction>(null, cancellationToken);
        }

        public virtual Task<GetCandidateOutputDTO> GetCandidateQueryAsync(GetCandidateFunction getCandidateFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetCandidateFunction, GetCandidateOutputDTO>(getCandidateFunction, blockParameter);
        }

        public virtual Task<GetCandidateOutputDTO> GetCandidateQueryAsync(BigInteger id, BlockParameter blockParameter = null)
        {
            var getCandidateFunction = new GetCandidateFunction();
                getCandidateFunction.Id = id;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetCandidateFunction, GetCandidateOutputDTO>(getCandidateFunction, blockParameter);
        }

        public Task<BigInteger> GetCandidateCountQueryAsync(GetCandidateCountFunction getCandidateCountFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetCandidateCountFunction, BigInteger>(getCandidateCountFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> GetCandidateCountQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetCandidateCountFunction, BigInteger>(null, blockParameter);
        }

        public virtual Task<GetWinnerOutputDTO> GetWinnerQueryAsync(GetWinnerFunction getWinnerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetWinnerFunction, GetWinnerOutputDTO>(getWinnerFunction, blockParameter);
        }

        public virtual Task<GetWinnerOutputDTO> GetWinnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetWinnerFunction, GetWinnerOutputDTO>(null, blockParameter);
        }

        public Task<bool> HasVotedQueryAsync(HasVotedFunction hasVotedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<HasVotedFunction, bool>(hasVotedFunction, blockParameter);
        }

        
        public virtual Task<bool> HasVotedQueryAsync(string returnValue1, BlockParameter blockParameter = null)
        {
            var hasVotedFunction = new HasVotedFunction();
                hasVotedFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<HasVotedFunction, bool>(hasVotedFunction, blockParameter);
        }

        public Task<string> OwnerQueryAsync(OwnerFunction ownerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(ownerFunction, blockParameter);
        }

        
        public virtual Task<string> OwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(null, blockParameter);
        }

        public Task<string> RegistryQueryAsync(RegistryFunction registryFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<RegistryFunction, string>(registryFunction, blockParameter);
        }

        
        public virtual Task<string> RegistryQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<RegistryFunction, string>(null, blockParameter);
        }

        public virtual Task<string> StartVotingRequestAsync(StartVotingFunction startVotingFunction)
        {
             return ContractHandler.SendRequestAsync(startVotingFunction);
        }

        public virtual Task<string> StartVotingRequestAsync()
        {
             return ContractHandler.SendRequestAsync<StartVotingFunction>();
        }

        public virtual Task<TransactionReceipt> StartVotingRequestAndWaitForReceiptAsync(StartVotingFunction startVotingFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(startVotingFunction, cancellationToken);
        }

        public virtual Task<TransactionReceipt> StartVotingRequestAndWaitForReceiptAsync(CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync<StartVotingFunction>(null, cancellationToken);
        }

        public virtual Task<string> TransferOwnershipRequestAsync(TransferOwnershipFunction transferOwnershipFunction)
        {
             return ContractHandler.SendRequestAsync(transferOwnershipFunction);
        }

        public virtual Task<TransactionReceipt> TransferOwnershipRequestAndWaitForReceiptAsync(TransferOwnershipFunction transferOwnershipFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferOwnershipFunction, cancellationToken);
        }

        public virtual Task<string> TransferOwnershipRequestAsync(string newOwner)
        {
            var transferOwnershipFunction = new TransferOwnershipFunction();
                transferOwnershipFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAsync(transferOwnershipFunction);
        }

        public virtual Task<TransactionReceipt> TransferOwnershipRequestAndWaitForReceiptAsync(string newOwner, CancellationTokenSource cancellationToken = null)
        {
            var transferOwnershipFunction = new TransferOwnershipFunction();
                transferOwnershipFunction.NewOwner = newOwner;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferOwnershipFunction, cancellationToken);
        }

        public virtual Task<string> VoteRequestAsync(VoteFunction voteFunction)
        {
             return ContractHandler.SendRequestAsync(voteFunction);
        }

        public virtual Task<TransactionReceipt> VoteRequestAndWaitForReceiptAsync(VoteFunction voteFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(voteFunction, cancellationToken);
        }

        public virtual Task<string> VoteRequestAsync(string voter, BigInteger candidateId)
        {
            var voteFunction = new VoteFunction();
                voteFunction.Voter = voter;
                voteFunction.CandidateId = candidateId;
            
             return ContractHandler.SendRequestAsync(voteFunction);
        }

        public virtual Task<TransactionReceipt> VoteRequestAndWaitForReceiptAsync(string voter, BigInteger candidateId, CancellationTokenSource cancellationToken = null)
        {
            var voteFunction = new VoteFunction();
                voteFunction.Voter = voter;
                voteFunction.CandidateId = candidateId;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(voteFunction, cancellationToken);
        }

        public Task<bool> VotingOpenQueryAsync(VotingOpenFunction votingOpenFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VotingOpenFunction, bool>(votingOpenFunction, blockParameter);
        }

        
        public virtual Task<bool> VotingOpenQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VotingOpenFunction, bool>(null, blockParameter);
        }

        public virtual Task<string> VoteWithSignatureRequestAsync(VoteWithSignatureFunction voteWithSignatureFunction)
        {
             return ContractHandler.SendRequestAsync(voteWithSignatureFunction);
        }

        public virtual Task<TransactionReceipt> VoteWithSignatureRequestAndWaitForReceiptAsync(VoteWithSignatureFunction voteWithSignatureFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(voteWithSignatureFunction, cancellationToken);
        }

        public virtual Task<string> VoteWithSignatureRequestAsync(string voter, BigInteger candidateId, BigInteger nonce, byte[] signature)
        {
            var voteWithSignatureFunction = new VoteWithSignatureFunction();
                voteWithSignatureFunction.Voter = voter;
                voteWithSignatureFunction.CandidateId = candidateId;
                voteWithSignatureFunction.SignatureNonce = nonce;
                voteWithSignatureFunction.Signature = signature;
            
             return ContractHandler.SendRequestAsync(voteWithSignatureFunction);
        }

        public virtual Task<TransactionReceipt> VoteWithSignatureRequestAndWaitForReceiptAsync(string voter, BigInteger candidateId, BigInteger nonce, byte[] signature, CancellationTokenSource cancellationToken = null)
        {
            var voteWithSignatureFunction = new VoteWithSignatureFunction();
                voteWithSignatureFunction.Voter = voter;
                voteWithSignatureFunction.CandidateId = candidateId;
                voteWithSignatureFunction.SignatureNonce = nonce;
                voteWithSignatureFunction.Signature = signature;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(voteWithSignatureFunction, cancellationToken);
        }

        public Task<BigInteger> NoncesQueryAsync(NoncesFunction noncesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NoncesFunction, BigInteger>(noncesFunction, blockParameter);
        }

        public virtual Task<BigInteger> NoncesQueryAsync(string voter, BlockParameter blockParameter = null)
        {
            var noncesFunction = new NoncesFunction();
                noncesFunction.Voter = voter;
            
            return ContractHandler.QueryAsync<NoncesFunction, BigInteger>(noncesFunction, blockParameter);
        }

        public Task<string> ZkVerifierQueryAsync(ZkVerifierFunction zkVerifierFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ZkVerifierFunction, string>(zkVerifierFunction, blockParameter);
        }

        
        public virtual Task<string> ZkVerifierQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ZkVerifierFunction, string>(null, blockParameter);
        }

        public override List<Type> GetAllFunctionTypes()
        {
            return new List<Type>
            {
                typeof(AddCandidateFunction),
                typeof(CandidatesFunction),
                typeof(EndVotingFunction),
                typeof(GetCandidateFunction),
                typeof(GetCandidateCountFunction),
                typeof(GetWinnerFunction),
                typeof(HasVotedFunction),
                typeof(OwnerFunction),
                typeof(RegistryFunction),
                typeof(StartVotingFunction),
                typeof(TransferOwnershipFunction),
                typeof(VoteFunction),
                typeof(VoteWithSignatureFunction),
                typeof(NoncesFunction),
                typeof(VotingOpenFunction),
                typeof(ZkVerifierFunction)
            };
        }

        public override List<Type> GetAllEventTypes()
        {
            return new List<Type>
            {
                typeof(CandidateAddedEventDTO),
                typeof(OwnershipTransferredEventDTO),
                typeof(VotedEventDTO),
                typeof(VotingEndedEventDTO),
                typeof(VotingStartedEventDTO)
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
