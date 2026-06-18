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
using VotingAPI.Services.Blockchain.Generated.BallotFactory.ContractDefinition;

namespace VotingAPI.Services.Blockchain.Generated.BallotFactory
{
    public partial class BallotFactoryService: BallotFactoryServiceBase
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.IWeb3 web3, BallotFactoryDeployment ballotFactoryDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<BallotFactoryDeployment>().SendRequestAndWaitForReceiptAsync(ballotFactoryDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.IWeb3 web3, BallotFactoryDeployment ballotFactoryDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<BallotFactoryDeployment>().SendRequestAsync(ballotFactoryDeployment);
        }

        public static async Task<BallotFactoryService> DeployContractAndGetServiceAsync(Nethereum.Web3.IWeb3 web3, BallotFactoryDeployment ballotFactoryDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, ballotFactoryDeployment, cancellationTokenSource);
            return new BallotFactoryService(web3, receipt.ContractAddress);
        }

        public BallotFactoryService(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

    }


    public partial class BallotFactoryServiceBase: ContractWeb3ServiceBase
    {

        public BallotFactoryServiceBase(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

        public Task<BigInteger> BallotCountQueryAsync(BallotCountFunction ballotCountFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BallotCountFunction, BigInteger>(ballotCountFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> BallotCountQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BallotCountFunction, BigInteger>(null, blockParameter);
        }

        public virtual Task<string> CreateBallotRequestAsync(CreateBallotFunction createBallotFunction)
        {
             return ContractHandler.SendRequestAsync(createBallotFunction);
        }

        public virtual Task<TransactionReceipt> CreateBallotRequestAndWaitForReceiptAsync(CreateBallotFunction createBallotFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createBallotFunction, cancellationToken);
        }

        public virtual Task<string> CreateBallotRequestAsync(string title, List<string> candidateNames)
        {
            var createBallotFunction = new CreateBallotFunction();
                createBallotFunction.Title = title;
                createBallotFunction.CandidateNames = candidateNames;
            
             return ContractHandler.SendRequestAsync(createBallotFunction);
        }

        public virtual Task<TransactionReceipt> CreateBallotRequestAndWaitForReceiptAsync(string title, List<string> candidateNames, CancellationTokenSource cancellationToken = null)
        {
            var createBallotFunction = new CreateBallotFunction();
                createBallotFunction.Title = title;
                createBallotFunction.CandidateNames = candidateNames;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(createBallotFunction, cancellationToken);
        }

        public virtual Task<GetAllBallotsOutputDTO> GetAllBallotsQueryAsync(GetAllBallotsFunction getAllBallotsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetAllBallotsFunction, GetAllBallotsOutputDTO>(getAllBallotsFunction, blockParameter);
        }

        public virtual Task<GetAllBallotsOutputDTO> GetAllBallotsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetAllBallotsFunction, GetAllBallotsOutputDTO>(null, blockParameter);
        }

        public Task<BigInteger> GetBallotCountQueryAsync(GetBallotCountFunction getBallotCountFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetBallotCountFunction, BigInteger>(getBallotCountFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> GetBallotCountQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetBallotCountFunction, BigInteger>(null, blockParameter);
        }

        public virtual Task<GetBallotInfoOutputDTO> GetBallotInfoQueryAsync(GetBallotInfoFunction getBallotInfoFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetBallotInfoFunction, GetBallotInfoOutputDTO>(getBallotInfoFunction, blockParameter);
        }

        public virtual Task<GetBallotInfoOutputDTO> GetBallotInfoQueryAsync(BigInteger ballotId, BlockParameter blockParameter = null)
        {
            var getBallotInfoFunction = new GetBallotInfoFunction();
                getBallotInfoFunction.BallotId = ballotId;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetBallotInfoFunction, GetBallotInfoOutputDTO>(getBallotInfoFunction, blockParameter);
        }

        public Task<string> GetVotingContractQueryAsync(GetVotingContractFunction getVotingContractFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetVotingContractFunction, string>(getVotingContractFunction, blockParameter);
        }

        
        public virtual Task<string> GetVotingContractQueryAsync(BigInteger ballotId, BlockParameter blockParameter = null)
        {
            var getVotingContractFunction = new GetVotingContractFunction();
                getVotingContractFunction.BallotId = ballotId;
            
            return ContractHandler.QueryAsync<GetVotingContractFunction, string>(getVotingContractFunction, blockParameter);
        }

        public Task<string> RegistryAddressQueryAsync(RegistryAddressFunction registryAddressFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<RegistryAddressFunction, string>(registryAddressFunction, blockParameter);
        }

        
        public virtual Task<string> RegistryAddressQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<RegistryAddressFunction, string>(null, blockParameter);
        }

        public Task<string> ZkVerifierAddressQueryAsync(ZkVerifierAddressFunction zkVerifierAddressFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ZkVerifierAddressFunction, string>(zkVerifierAddressFunction, blockParameter);
        }

        
        public virtual Task<string> ZkVerifierAddressQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ZkVerifierAddressFunction, string>(null, blockParameter);
        }

        public override List<Type> GetAllFunctionTypes()
        {
            return new List<Type>
            {
                typeof(BallotCountFunction),
                typeof(CreateBallotFunction),
                typeof(GetAllBallotsFunction),
                typeof(GetBallotCountFunction),
                typeof(GetBallotInfoFunction),
                typeof(GetVotingContractFunction),
                typeof(RegistryAddressFunction),
                typeof(ZkVerifierAddressFunction)
            };
        }

        public override List<Type> GetAllEventTypes()
        {
            return new List<Type>
            {
                typeof(BallotCreatedEventDTO)
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
