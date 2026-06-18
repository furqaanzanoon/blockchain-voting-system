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
using VotingAPI.Services.Blockchain.Generated.ResultAggregator.ContractDefinition;

namespace VotingAPI.Services.Blockchain.Generated.ResultAggregator
{
    public partial class ResultAggregatorService: ResultAggregatorServiceBase
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.IWeb3 web3, ResultAggregatorDeployment resultAggregatorDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<ResultAggregatorDeployment>().SendRequestAndWaitForReceiptAsync(resultAggregatorDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.IWeb3 web3, ResultAggregatorDeployment resultAggregatorDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<ResultAggregatorDeployment>().SendRequestAsync(resultAggregatorDeployment);
        }

        public static async Task<ResultAggregatorService> DeployContractAndGetServiceAsync(Nethereum.Web3.IWeb3 web3, ResultAggregatorDeployment resultAggregatorDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, resultAggregatorDeployment, cancellationTokenSource);
            return new ResultAggregatorService(web3, receipt.ContractAddress);
        }

        public ResultAggregatorService(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

    }


    public partial class ResultAggregatorServiceBase: ContractWeb3ServiceBase
    {

        public ResultAggregatorServiceBase(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

        public Task<string> FactoryQueryAsync(FactoryFunction factoryFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<FactoryFunction, string>(factoryFunction, blockParameter);
        }

        
        public virtual Task<string> FactoryQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<FactoryFunction, string>(null, blockParameter);
        }

        public virtual Task<GetActiveBallotsOutputDTO> GetActiveBallotsQueryAsync(GetActiveBallotsFunction getActiveBallotsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetActiveBallotsFunction, GetActiveBallotsOutputDTO>(getActiveBallotsFunction, blockParameter);
        }

        public virtual Task<GetActiveBallotsOutputDTO> GetActiveBallotsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetActiveBallotsFunction, GetActiveBallotsOutputDTO>(null, blockParameter);
        }

        public virtual Task<GetAllResultsOutputDTO> GetAllResultsQueryAsync(GetAllResultsFunction getAllResultsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetAllResultsFunction, GetAllResultsOutputDTO>(getAllResultsFunction, blockParameter);
        }

        public virtual Task<GetAllResultsOutputDTO> GetAllResultsQueryAsync(BigInteger offset, BigInteger limit, BlockParameter blockParameter = null)
        {
            var getAllResultsFunction = new GetAllResultsFunction();
                getAllResultsFunction.Offset = offset;
                getAllResultsFunction.Limit = limit;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetAllResultsFunction, GetAllResultsOutputDTO>(getAllResultsFunction, blockParameter);
        }

        public virtual Task<GetBallotResultOutputDTO> GetBallotResultQueryAsync(GetBallotResultFunction getBallotResultFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetBallotResultFunction, GetBallotResultOutputDTO>(getBallotResultFunction, blockParameter);
        }

        public virtual Task<GetBallotResultOutputDTO> GetBallotResultQueryAsync(BigInteger ballotId, BlockParameter blockParameter = null)
        {
            var getBallotResultFunction = new GetBallotResultFunction();
                getBallotResultFunction.BallotId = ballotId;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetBallotResultFunction, GetBallotResultOutputDTO>(getBallotResultFunction, blockParameter);
        }

        public Task<BigInteger> GetGlobalTotalVotesQueryAsync(GetGlobalTotalVotesFunction getGlobalTotalVotesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetGlobalTotalVotesFunction, BigInteger>(getGlobalTotalVotesFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> GetGlobalTotalVotesQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetGlobalTotalVotesFunction, BigInteger>(null, blockParameter);
        }

        public virtual Task<GetLeaderboardOutputDTO> GetLeaderboardQueryAsync(GetLeaderboardFunction getLeaderboardFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetLeaderboardFunction, GetLeaderboardOutputDTO>(getLeaderboardFunction, blockParameter);
        }

        public virtual Task<GetLeaderboardOutputDTO> GetLeaderboardQueryAsync(BigInteger offset, BigInteger limit, BlockParameter blockParameter = null)
        {
            var getLeaderboardFunction = new GetLeaderboardFunction();
                getLeaderboardFunction.Offset = offset;
                getLeaderboardFunction.Limit = limit;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetLeaderboardFunction, GetLeaderboardOutputDTO>(getLeaderboardFunction, blockParameter);
        }

        public virtual Task<GetParticipationShareOutputDTO> GetParticipationShareQueryAsync(GetParticipationShareFunction getParticipationShareFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetParticipationShareFunction, GetParticipationShareOutputDTO>(getParticipationShareFunction, blockParameter);
        }

        public virtual Task<GetParticipationShareOutputDTO> GetParticipationShareQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetParticipationShareFunction, GetParticipationShareOutputDTO>(null, blockParameter);
        }

        public virtual Task<GetVotesByCandidateOutputDTO> GetVotesByCandidateQueryAsync(GetVotesByCandidateFunction getVotesByCandidateFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetVotesByCandidateFunction, GetVotesByCandidateOutputDTO>(getVotesByCandidateFunction, blockParameter);
        }

        public virtual Task<GetVotesByCandidateOutputDTO> GetVotesByCandidateQueryAsync(string name, BlockParameter blockParameter = null)
        {
            var getVotesByCandidateFunction = new GetVotesByCandidateFunction();
                getVotesByCandidateFunction.Name = name;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetVotesByCandidateFunction, GetVotesByCandidateOutputDTO>(getVotesByCandidateFunction, blockParameter);
        }

        public override List<Type> GetAllFunctionTypes()
        {
            return new List<Type>
            {
                typeof(FactoryFunction),
                typeof(GetActiveBallotsFunction),
                typeof(GetAllResultsFunction),
                typeof(GetBallotResultFunction),
                typeof(GetGlobalTotalVotesFunction),
                typeof(GetLeaderboardFunction),
                typeof(GetParticipationShareFunction),
                typeof(GetVotesByCandidateFunction)
            };
        }

        public override List<Type> GetAllEventTypes()
        {
            return new List<Type>
            {

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
