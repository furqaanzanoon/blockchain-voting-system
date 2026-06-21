using Nethereum.Contracts;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Numerics;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using VotingAPI.Models.DTOs.Result;
using VotingAPI.Models.DTOs.Vote;
using VotingAPI.Services.Blockchain.Generated.AccessControl;
using VotingAPI.Services.Blockchain.Generated.BallotFactory;
using VotingAPI.Services.Blockchain.Generated.BallotFactory.ContractDefinition;
using VotingAPI.Services.Blockchain.Generated.VoterRegistry;
using VotingAPI.Services.Blockchain.Generated.Voting;
using VotingAPI.Services.Blockchain.Generated.Voting.ContractDefinition;
using VotingAPI.Services.Interfaces;

namespace VotingAPI.Services
{
    public class BlockchainService : IBlockchainService
    {
        private readonly Web3 web3;
        private readonly IMemoryCache cache;
        private readonly string adminWalletAddress;
        private readonly string accessControlAddress;
        private readonly string zkVerifierAddress;
        private readonly string voterRegistryAddress;
        private readonly string ballotFactoryAddress;
        private readonly string resultAggregatorAddress;

        public BlockchainService(IConfiguration config, IMemoryCache cache)
        {
            this.cache = cache;
            var privateKey = config["BlockchainSettings:AdminPrivateKey"]!;
            var account = new Account(privateKey);
            adminWalletAddress = account.Address;
            web3 = new Web3(account, config["BlockchainSettings:NodeUrl"]);
            accessControlAddress = config["BlockchainSettings:AccessControlAddress"] ?? throw new InvalidOperationException("AccessControlAddress is missing");
            zkVerifierAddress = config["BlockchainSettings:ZKVerifierAddress"] ?? throw new InvalidOperationException("ZKVerifierAddress is missing");
            voterRegistryAddress = config["BlockchainSettings:VoterRegistryAddress"] ?? throw new InvalidOperationException("VoterRegistryAddress is missing");
            ballotFactoryAddress = config["BlockchainSettings:BallotFactoryAddress"] ?? throw new ArgumentException("BallotFactoryAddress is missing");
            resultAggregatorAddress = config["BlockchainSettings:ResultAggregatorAddress"] ?? throw new ArgumentException("ResultAggregatorAddress is missing");
        }

        public async Task<string> CreateNewElectionAsync(string title, List<string> candidateNames)
        {
            var ballotFactoryService = new BallotFactoryService(web3, ballotFactoryAddress);

            var createBallotFunction = new CreateBallotFunction
            {
                Title = title,
                CandidateNames = candidateNames
            };

            // 1. Execute the transaction on the blockchain
            var receipt = await ballotFactoryService.CreateBallotRequestAndWaitForReceiptAsync(createBallotFunction);

            // 2. Decode the BallotCreated event to get the new contract address
            var ballotCreatedEvent = receipt.DecodeAllEvents<BallotCreatedEventDTO>();
            var votingContractAddress = ballotCreatedEvent.FirstOrDefault()?.Event.ContractAddress;

            if (string.IsNullOrEmpty(votingContractAddress))
                throw new InvalidOperationException("Failed to retrieve the new election contract address from the blockchain events.");

            return votingContractAddress;
        }

        public async Task StartVotingAsync(string votingContractAddress)
        {
            var votingService = new VotingService(web3, votingContractAddress);
            await votingService.StartVotingRequestAndWaitForReceiptAsync();
            cache.Remove($"ElectionResults_{votingContractAddress}");
        }

        public async Task EndVotingAsync(string votingContractAddress)
        {
            var votingService = new VotingService(web3, votingContractAddress);
            await votingService.EndVotingRequestAndWaitForReceiptAsync();
            cache.Remove($"ElectionResults_{votingContractAddress}");
        }

        public async Task GrantBallotAdminRoleAsync(string votingContractAddress)
        {
            var accessControlService = new AccessControlService(web3, accessControlAddress);

            var role = await accessControlService.BallotAdminQueryAsync();

            await accessControlService.GrantBallotRoleRequestAndWaitForReceiptAsync(votingContractAddress, role, adminWalletAddress);
        }

        public async Task SetEligibilityAsync(string votingContractAddress, string walletAddress)
        {
            var voterRegistryService = new VoterRegistryService(web3, voterRegistryAddress);

            await voterRegistryService.SetEligibilityRequestAndWaitForReceiptAsync(votingContractAddress, walletAddress, true);
        }

        public async Task<List<CandidateResultDTO>> GetResultsAsync(string votingContractAddress)
        {
            var cacheKey = $"ElectionResults_{votingContractAddress}";
            if (cache.TryGetValue(cacheKey, out List<CandidateResultDTO>? cachedResults) && cachedResults != null)
            {
                return cachedResults;
            }

            var votingService = new VotingService(web3, votingContractAddress);
            
            var count = await votingService.GetCandidateCountQueryAsync();
            var intCount = (int)count;

            var tasks = new Task<GetCandidateOutputDTO>[intCount];
            for (int i = 0; i < intCount; i++)
            {
                tasks[i] = votingService.GetCandidateQueryAsync(i);
            }

            var candidates = await Task.WhenAll(tasks);
            var results = candidates.Select(candidate => new CandidateResultDTO
            {
                CandidateName = candidate.Name,
                VoteCount = (long)candidate.VoteCount
            }).ToList();

            // Cache closed elections longer, active elections for 30s
            bool isOpen = await votingService.VotingOpenQueryAsync();
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = isOpen ? TimeSpan.FromSeconds(30) : TimeSpan.FromHours(1)
            };

            cache.Set(cacheKey, results, options);

            return results;
        }

        public async Task<(string TxHash, long BlockNumber)> CastVoteAsync(string votingContractAddress, string voterAddress, int candidateIndex)
        {
            var votingService = new VotingService(web3, votingContractAddress);
            var receipt = await votingService.VoteRequestAndWaitForReceiptAsync(voterAddress, candidateIndex);

            cache.Remove($"ElectionResults_{votingContractAddress}");

            return (receipt.TransactionHash, (long)receipt.BlockNumber.Value);
        }

        public async Task<long> GetVoterNonceAsync(string votingContractAddress, string voterAddress)
        {
            var votingService = new VotingService(web3, votingContractAddress);
            var nonce = await votingService.NoncesQueryAsync(voterAddress);
            return (long)nonce;
        }

        public async Task<(string TxHash, long BlockNumber)> CastVoteWithSignatureAsync(string votingContractAddress, string voterAddress, int candidateIndex, long nonce, string signature)
        {
            byte[] signatureBytes = Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions.HexToByteArray(signature);

            // Normalize v to 27 or 28 if returned as 0 or 1 by MetaMask/client
            if (signatureBytes.Length == 65 && signatureBytes[64] < 27)
            {
                signatureBytes[64] += 27;
            }

            var votingService = new VotingService(web3, votingContractAddress);
            var receipt = await votingService.VoteWithSignatureRequestAndWaitForReceiptAsync(
                voterAddress,
                new System.Numerics.BigInteger(candidateIndex),
                new System.Numerics.BigInteger(nonce),
                signatureBytes
            );

            cache.Remove($"ElectionResults_{votingContractAddress}");

            return (receipt.TransactionHash, (long)receipt.BlockNumber.Value);
        }

        public async Task<(string TxHash, long BlockNumber)> VerifyZkVoteAsync(
            VotingAPI.Services.Blockchain.Generated.ZKVerifier.ContractDefinition.Proof proof, 
            VotingAPI.Services.Blockchain.Generated.ZKVerifier.ContractDefinition.PublicSignals signals
        )
        {
            var zkVerifierService = new VotingAPI.Services.Blockchain.Generated.ZKVerifier.ZKVerifierService(web3, zkVerifierAddress);
            var receipt = await zkVerifierService.VerifyAndVoteRequestAndWaitForReceiptAsync(proof, signals);
            return (receipt.TransactionHash, (long)receipt.BlockNumber.Value);
        }

        public async Task SetBallotRootAsync(System.Numerics.BigInteger ballotId, System.Numerics.BigInteger merkleRoot)
        {
            var zkVerifierService = new VotingAPI.Services.Blockchain.Generated.ZKVerifier.ZKVerifierService(web3, zkVerifierAddress);
            await zkVerifierService.SetBallotRootRequestAndWaitForReceiptAsync(ballotId, merkleRoot);
        }
    }
}