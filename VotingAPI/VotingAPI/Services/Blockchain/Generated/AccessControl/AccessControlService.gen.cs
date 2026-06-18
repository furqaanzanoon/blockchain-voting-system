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
using VotingAPI.Services.Blockchain.Generated.AccessControl.ContractDefinition;

namespace VotingAPI.Services.Blockchain.Generated.AccessControl
{
    public partial class AccessControlService: AccessControlServiceBase
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.IWeb3 web3, AccessControlDeployment accessControlDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<AccessControlDeployment>().SendRequestAndWaitForReceiptAsync(accessControlDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.IWeb3 web3, AccessControlDeployment accessControlDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<AccessControlDeployment>().SendRequestAsync(accessControlDeployment);
        }

        public static async Task<AccessControlService> DeployContractAndGetServiceAsync(Nethereum.Web3.IWeb3 web3, AccessControlDeployment accessControlDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, accessControlDeployment, cancellationTokenSource);
            return new AccessControlService(web3, receipt.ContractAddress);
        }

        public AccessControlService(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

    }


    public partial class AccessControlServiceBase: ContractWeb3ServiceBase
    {

        public AccessControlServiceBase(Nethereum.Web3.IWeb3 web3, string contractAddress) : base(web3, contractAddress)
        {
        }

        public Task<byte[]> AdminQueryAsync(AdminFunction adminFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AdminFunction, byte[]>(adminFunction, blockParameter);
        }

        
        public virtual Task<byte[]> AdminQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AdminFunction, byte[]>(null, blockParameter);
        }

        public Task<byte[]> AuditorQueryAsync(AuditorFunction auditorFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AuditorFunction, byte[]>(auditorFunction, blockParameter);
        }

        
        public virtual Task<byte[]> AuditorQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<AuditorFunction, byte[]>(null, blockParameter);
        }

        public Task<byte[]> BallotAdminQueryAsync(BallotAdminFunction ballotAdminFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BallotAdminFunction, byte[]>(ballotAdminFunction, blockParameter);
        }

        
        public virtual Task<byte[]> BallotAdminQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BallotAdminFunction, byte[]>(null, blockParameter);
        }

        public Task<byte[]> SuperAdminQueryAsync(SuperAdminFunction superAdminFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SuperAdminFunction, byte[]>(superAdminFunction, blockParameter);
        }

        
        public virtual Task<byte[]> SuperAdminQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SuperAdminFunction, byte[]>(null, blockParameter);
        }

        public Task<byte[]> VoterQueryAsync(VoterFunction voterFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VoterFunction, byte[]>(voterFunction, blockParameter);
        }

        
        public virtual Task<byte[]> VoterQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<VoterFunction, byte[]>(null, blockParameter);
        }

        public virtual Task<string> BatchGrantRoleRequestAsync(BatchGrantRoleFunction batchGrantRoleFunction)
        {
             return ContractHandler.SendRequestAsync(batchGrantRoleFunction);
        }

        public virtual Task<TransactionReceipt> BatchGrantRoleRequestAndWaitForReceiptAsync(BatchGrantRoleFunction batchGrantRoleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchGrantRoleFunction, cancellationToken);
        }

        public virtual Task<string> BatchGrantRoleRequestAsync(byte[] role, List<string> accounts)
        {
            var batchGrantRoleFunction = new BatchGrantRoleFunction();
                batchGrantRoleFunction.Role = role;
                batchGrantRoleFunction.Accounts = accounts;
            
             return ContractHandler.SendRequestAsync(batchGrantRoleFunction);
        }

        public virtual Task<TransactionReceipt> BatchGrantRoleRequestAndWaitForReceiptAsync(byte[] role, List<string> accounts, CancellationTokenSource cancellationToken = null)
        {
            var batchGrantRoleFunction = new BatchGrantRoleFunction();
                batchGrantRoleFunction.Role = role;
                batchGrantRoleFunction.Accounts = accounts;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchGrantRoleFunction, cancellationToken);
        }

        public virtual Task<string> BatchRevokeRoleRequestAsync(BatchRevokeRoleFunction batchRevokeRoleFunction)
        {
             return ContractHandler.SendRequestAsync(batchRevokeRoleFunction);
        }

        public virtual Task<TransactionReceipt> BatchRevokeRoleRequestAndWaitForReceiptAsync(BatchRevokeRoleFunction batchRevokeRoleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchRevokeRoleFunction, cancellationToken);
        }

        public virtual Task<string> BatchRevokeRoleRequestAsync(byte[] role, List<string> accounts)
        {
            var batchRevokeRoleFunction = new BatchRevokeRoleFunction();
                batchRevokeRoleFunction.Role = role;
                batchRevokeRoleFunction.Accounts = accounts;
            
             return ContractHandler.SendRequestAsync(batchRevokeRoleFunction);
        }

        public virtual Task<TransactionReceipt> BatchRevokeRoleRequestAndWaitForReceiptAsync(byte[] role, List<string> accounts, CancellationTokenSource cancellationToken = null)
        {
            var batchRevokeRoleFunction = new BatchRevokeRoleFunction();
                batchRevokeRoleFunction.Role = role;
                batchRevokeRoleFunction.Accounts = accounts;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(batchRevokeRoleFunction, cancellationToken);
        }

        public Task<byte[]> GetRoleAdminQueryAsync(GetRoleAdminFunction getRoleAdminFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetRoleAdminFunction, byte[]>(getRoleAdminFunction, blockParameter);
        }

        
        public virtual Task<byte[]> GetRoleAdminQueryAsync(byte[] role, BlockParameter blockParameter = null)
        {
            var getRoleAdminFunction = new GetRoleAdminFunction();
                getRoleAdminFunction.Role = role;
            
            return ContractHandler.QueryAsync<GetRoleAdminFunction, byte[]>(getRoleAdminFunction, blockParameter);
        }

        public virtual Task<GetRoleHistoryOutputDTO> GetRoleHistoryQueryAsync(GetRoleHistoryFunction getRoleHistoryFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetRoleHistoryFunction, GetRoleHistoryOutputDTO>(getRoleHistoryFunction, blockParameter);
        }

        public virtual Task<GetRoleHistoryOutputDTO> GetRoleHistoryQueryAsync(BigInteger offset, BigInteger limit, BlockParameter blockParameter = null)
        {
            var getRoleHistoryFunction = new GetRoleHistoryFunction();
                getRoleHistoryFunction.Offset = offset;
                getRoleHistoryFunction.Limit = limit;
            
            return ContractHandler.QueryDeserializingToObjectAsync<GetRoleHistoryFunction, GetRoleHistoryOutputDTO>(getRoleHistoryFunction, blockParameter);
        }

        public Task<BigInteger> GetRoleMemberCountQueryAsync(GetRoleMemberCountFunction getRoleMemberCountFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetRoleMemberCountFunction, BigInteger>(getRoleMemberCountFunction, blockParameter);
        }

        
        public virtual Task<BigInteger> GetRoleMemberCountQueryAsync(byte[] role, BlockParameter blockParameter = null)
        {
            var getRoleMemberCountFunction = new GetRoleMemberCountFunction();
                getRoleMemberCountFunction.Role = role;
            
            return ContractHandler.QueryAsync<GetRoleMemberCountFunction, BigInteger>(getRoleMemberCountFunction, blockParameter);
        }

        public Task<List<byte[]>> GetRolesOfQueryAsync(GetRolesOfFunction getRolesOfFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetRolesOfFunction, List<byte[]>>(getRolesOfFunction, blockParameter);
        }

        
        public virtual Task<List<byte[]>> GetRolesOfQueryAsync(string account, BlockParameter blockParameter = null)
        {
            var getRolesOfFunction = new GetRolesOfFunction();
                getRolesOfFunction.Account = account;
            
            return ContractHandler.QueryAsync<GetRolesOfFunction, List<byte[]>>(getRolesOfFunction, blockParameter);
        }

        public virtual Task<string> GrantBallotRoleRequestAsync(GrantBallotRoleFunction grantBallotRoleFunction)
        {
             return ContractHandler.SendRequestAsync(grantBallotRoleFunction);
        }

        public virtual Task<TransactionReceipt> GrantBallotRoleRequestAndWaitForReceiptAsync(GrantBallotRoleFunction grantBallotRoleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(grantBallotRoleFunction, cancellationToken);
        }

        public virtual Task<string> GrantBallotRoleRequestAsync(string ballot, byte[] role, string account)
        {
            var grantBallotRoleFunction = new GrantBallotRoleFunction();
                grantBallotRoleFunction.Ballot = ballot;
                grantBallotRoleFunction.Role = role;
                grantBallotRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAsync(grantBallotRoleFunction);
        }

        public virtual Task<TransactionReceipt> GrantBallotRoleRequestAndWaitForReceiptAsync(string ballot, byte[] role, string account, CancellationTokenSource cancellationToken = null)
        {
            var grantBallotRoleFunction = new GrantBallotRoleFunction();
                grantBallotRoleFunction.Ballot = ballot;
                grantBallotRoleFunction.Role = role;
                grantBallotRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(grantBallotRoleFunction, cancellationToken);
        }

        public virtual Task<string> GrantRoleRequestAsync(GrantRoleFunction grantRoleFunction)
        {
             return ContractHandler.SendRequestAsync(grantRoleFunction);
        }

        public virtual Task<TransactionReceipt> GrantRoleRequestAndWaitForReceiptAsync(GrantRoleFunction grantRoleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(grantRoleFunction, cancellationToken);
        }

        public virtual Task<string> GrantRoleRequestAsync(byte[] role, string account)
        {
            var grantRoleFunction = new GrantRoleFunction();
                grantRoleFunction.Role = role;
                grantRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAsync(grantRoleFunction);
        }

        public virtual Task<TransactionReceipt> GrantRoleRequestAndWaitForReceiptAsync(byte[] role, string account, CancellationTokenSource cancellationToken = null)
        {
            var grantRoleFunction = new GrantRoleFunction();
                grantRoleFunction.Role = role;
                grantRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(grantRoleFunction, cancellationToken);
        }

        public Task<bool> HasBallotRoleQueryAsync(HasBallotRoleFunction hasBallotRoleFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<HasBallotRoleFunction, bool>(hasBallotRoleFunction, blockParameter);
        }

        
        public virtual Task<bool> HasBallotRoleQueryAsync(string ballot, byte[] role, string account, BlockParameter blockParameter = null)
        {
            var hasBallotRoleFunction = new HasBallotRoleFunction();
                hasBallotRoleFunction.Ballot = ballot;
                hasBallotRoleFunction.Role = role;
                hasBallotRoleFunction.Account = account;
            
            return ContractHandler.QueryAsync<HasBallotRoleFunction, bool>(hasBallotRoleFunction, blockParameter);
        }

        public Task<bool> HasRoleQueryAsync(HasRoleFunction hasRoleFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<HasRoleFunction, bool>(hasRoleFunction, blockParameter);
        }

        
        public virtual Task<bool> HasRoleQueryAsync(byte[] role, string account, BlockParameter blockParameter = null)
        {
            var hasRoleFunction = new HasRoleFunction();
                hasRoleFunction.Role = role;
                hasRoleFunction.Account = account;
            
            return ContractHandler.QueryAsync<HasRoleFunction, bool>(hasRoleFunction, blockParameter);
        }

        public Task<bool> IsAuthorizedQueryAsync(IsAuthorizedFunction isAuthorizedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsAuthorizedFunction, bool>(isAuthorizedFunction, blockParameter);
        }

        
        public virtual Task<bool> IsAuthorizedQueryAsync(string ballot, byte[] role, string account, BlockParameter blockParameter = null)
        {
            var isAuthorizedFunction = new IsAuthorizedFunction();
                isAuthorizedFunction.Ballot = ballot;
                isAuthorizedFunction.Role = role;
                isAuthorizedFunction.Account = account;
            
            return ContractHandler.QueryAsync<IsAuthorizedFunction, bool>(isAuthorizedFunction, blockParameter);
        }

        public virtual Task<string> RenounceRoleRequestAsync(RenounceRoleFunction renounceRoleFunction)
        {
             return ContractHandler.SendRequestAsync(renounceRoleFunction);
        }

        public virtual Task<TransactionReceipt> RenounceRoleRequestAndWaitForReceiptAsync(RenounceRoleFunction renounceRoleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(renounceRoleFunction, cancellationToken);
        }

        public virtual Task<string> RenounceRoleRequestAsync(byte[] role)
        {
            var renounceRoleFunction = new RenounceRoleFunction();
                renounceRoleFunction.Role = role;
            
             return ContractHandler.SendRequestAsync(renounceRoleFunction);
        }

        public virtual Task<TransactionReceipt> RenounceRoleRequestAndWaitForReceiptAsync(byte[] role, CancellationTokenSource cancellationToken = null)
        {
            var renounceRoleFunction = new RenounceRoleFunction();
                renounceRoleFunction.Role = role;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(renounceRoleFunction, cancellationToken);
        }

        public virtual Task<string> RevokeBallotRoleRequestAsync(RevokeBallotRoleFunction revokeBallotRoleFunction)
        {
             return ContractHandler.SendRequestAsync(revokeBallotRoleFunction);
        }

        public virtual Task<TransactionReceipt> RevokeBallotRoleRequestAndWaitForReceiptAsync(RevokeBallotRoleFunction revokeBallotRoleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeBallotRoleFunction, cancellationToken);
        }

        public virtual Task<string> RevokeBallotRoleRequestAsync(string ballot, byte[] role, string account)
        {
            var revokeBallotRoleFunction = new RevokeBallotRoleFunction();
                revokeBallotRoleFunction.Ballot = ballot;
                revokeBallotRoleFunction.Role = role;
                revokeBallotRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAsync(revokeBallotRoleFunction);
        }

        public virtual Task<TransactionReceipt> RevokeBallotRoleRequestAndWaitForReceiptAsync(string ballot, byte[] role, string account, CancellationTokenSource cancellationToken = null)
        {
            var revokeBallotRoleFunction = new RevokeBallotRoleFunction();
                revokeBallotRoleFunction.Ballot = ballot;
                revokeBallotRoleFunction.Role = role;
                revokeBallotRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeBallotRoleFunction, cancellationToken);
        }

        public virtual Task<string> RevokeRoleRequestAsync(RevokeRoleFunction revokeRoleFunction)
        {
             return ContractHandler.SendRequestAsync(revokeRoleFunction);
        }

        public virtual Task<TransactionReceipt> RevokeRoleRequestAndWaitForReceiptAsync(RevokeRoleFunction revokeRoleFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeRoleFunction, cancellationToken);
        }

        public virtual Task<string> RevokeRoleRequestAsync(byte[] role, string account)
        {
            var revokeRoleFunction = new RevokeRoleFunction();
                revokeRoleFunction.Role = role;
                revokeRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAsync(revokeRoleFunction);
        }

        public virtual Task<TransactionReceipt> RevokeRoleRequestAndWaitForReceiptAsync(byte[] role, string account, CancellationTokenSource cancellationToken = null)
        {
            var revokeRoleFunction = new RevokeRoleFunction();
                revokeRoleFunction.Role = role;
                revokeRoleFunction.Account = account;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(revokeRoleFunction, cancellationToken);
        }

        public virtual Task<RoleHistoryOutputDTO> RoleHistoryQueryAsync(RoleHistoryFunction roleHistoryFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<RoleHistoryFunction, RoleHistoryOutputDTO>(roleHistoryFunction, blockParameter);
        }

        public virtual Task<RoleHistoryOutputDTO> RoleHistoryQueryAsync(BigInteger returnValue1, BlockParameter blockParameter = null)
        {
            var roleHistoryFunction = new RoleHistoryFunction();
                roleHistoryFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryDeserializingToObjectAsync<RoleHistoryFunction, RoleHistoryOutputDTO>(roleHistoryFunction, blockParameter);
        }

        public virtual Task<string> SetRoleAdminRequestAsync(SetRoleAdminFunction setRoleAdminFunction)
        {
             return ContractHandler.SendRequestAsync(setRoleAdminFunction);
        }

        public virtual Task<TransactionReceipt> SetRoleAdminRequestAndWaitForReceiptAsync(SetRoleAdminFunction setRoleAdminFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setRoleAdminFunction, cancellationToken);
        }

        public virtual Task<string> SetRoleAdminRequestAsync(byte[] role, byte[] adminRole)
        {
            var setRoleAdminFunction = new SetRoleAdminFunction();
                setRoleAdminFunction.Role = role;
                setRoleAdminFunction.AdminRole = adminRole;
            
             return ContractHandler.SendRequestAsync(setRoleAdminFunction);
        }

        public virtual Task<TransactionReceipt> SetRoleAdminRequestAndWaitForReceiptAsync(byte[] role, byte[] adminRole, CancellationTokenSource cancellationToken = null)
        {
            var setRoleAdminFunction = new SetRoleAdminFunction();
                setRoleAdminFunction.Role = role;
                setRoleAdminFunction.AdminRole = adminRole;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setRoleAdminFunction, cancellationToken);
        }

        public override List<Type> GetAllFunctionTypes()
        {
            return new List<Type>
            {
                typeof(AdminFunction),
                typeof(AuditorFunction),
                typeof(BallotAdminFunction),
                typeof(SuperAdminFunction),
                typeof(VoterFunction),
                typeof(BatchGrantRoleFunction),
                typeof(BatchRevokeRoleFunction),
                typeof(GetRoleAdminFunction),
                typeof(GetRoleHistoryFunction),
                typeof(GetRoleMemberCountFunction),
                typeof(GetRolesOfFunction),
                typeof(GrantBallotRoleFunction),
                typeof(GrantRoleFunction),
                typeof(HasBallotRoleFunction),
                typeof(HasRoleFunction),
                typeof(IsAuthorizedFunction),
                typeof(RenounceRoleFunction),
                typeof(RevokeBallotRoleFunction),
                typeof(RevokeRoleFunction),
                typeof(RoleHistoryFunction),
                typeof(SetRoleAdminFunction)
            };
        }

        public override List<Type> GetAllEventTypes()
        {
            return new List<Type>
            {
                typeof(BallotRoleGrantedEventDTO),
                typeof(BallotRoleRevokedEventDTO),
                typeof(RoleAdminChangedEventDTO),
                typeof(RoleGrantedEventDTO),
                typeof(RoleRevokedEventDTO)
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
