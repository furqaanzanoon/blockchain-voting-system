using Microsoft.EntityFrameworkCore;
using Nethereum.Util;
using VotingAPI.Data;
using VotingAPI.Models.DTOs.User;
using VotingAPI.Models.Enums;
using VotingAPI.Services.Interfaces;

namespace VotingAPI.Services
{
    public class UserService : IUserService
    {
        private readonly VotingDbContext dbContext;

        public UserService(VotingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> ConnectWallet(Guid userId, string ethAddress)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId) ?? throw new KeyNotFoundException("User not found");

            var wallectConnected = await dbContext.Users.AnyAsync(u => u.EthAddress == ethAddress && u.UserId != userId);

            if (wallectConnected)
                throw new InvalidOperationException("Wallet already connected");

            if (!AddressUtil.Current.IsValidEthereumAddressHexFormat(ethAddress))
                throw new ArgumentException("Invalid Ethereum address");

            user.EthAddress = ethAddress;
            await dbContext.SaveChangesAsync();

            return "Wallet connected successfully";
        }

        public async Task<List<UserResponseDTO>> GetAllUsers()
        {
            // Fetches only users (voters), not admins, election officers, and parties
            var usersRoles = await dbContext.Users.AsNoTracking().Where(u => u.Role == UserRole.Voter).Select(u => new UserResponseDTO
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                EthAddress = u.EthAddress,
                Role = u.Role,
                IsApproved = u.IsApproved,
                IsVerified = u.IsVerified,
                PartyAffiliation = u.PartyAffiliation
            }).ToListAsync() ?? throw new KeyNotFoundException("No user found");

            return usersRoles;
        }

        public async Task<string> ChangePassword(Guid userId, ChangePasswordRequestDTO changePasswordRequestDTO)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId) ?? throw new KeyNotFoundException("User not found");

            if (!BCrypt.Net.BCrypt.Verify(changePasswordRequestDTO.CurrentPassword, user.PasswordHash))
                throw new InvalidOperationException("Current password is incorrect.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordRequestDTO.NewPassword);
            await dbContext.SaveChangesAsync();

            return "Password changed successfully.";
        }

        public async Task<List<UserResponseDTO>> GetPendingUsers(Guid currentUserId, UserRole currentRole)
        {
            var query = dbContext.Users.AsNoTracking().Where(u => !u.IsApproved && u.IsVerified);

            if (currentRole == UserRole.Party)
            {
                var partyUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == currentUserId)
                    ?? throw new KeyNotFoundException("Party user not found.");

                query = query.Where(u => u.Role == UserRole.Candidate && u.PartyAffiliation == partyUser.FullName);
            }
            else if (currentRole == UserRole.ElectionOfficer)
            {
                query = query.Where(u => u.Role == UserRole.Voter || (u.Role == UserRole.Candidate && u.PartyAffiliation == "Independent"));
            }
            else if (currentRole == UserRole.Admin)
            {
                query = query.Where(u => u.Role == UserRole.Voter || u.Role == UserRole.ElectionOfficer || (u.Role == UserRole.Candidate && u.PartyAffiliation == "Independent"));
            }
            else
            {
                throw new UnauthorizedAccessException("Unauthorized role for checking pending users.");
            }

            return await query.Select(u => new UserResponseDTO
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                EthAddress = u.EthAddress,
                Role = u.Role,
                IsApproved = u.IsApproved,
                IsVerified = u.IsVerified,
                PartyAffiliation = u.PartyAffiliation
            }).ToListAsync();
        }

        public async Task ApproveUser(Guid currentUserId, UserRole currentRole, Guid targetUserId)
        {
            var targetUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == targetUserId) 
                ?? throw new KeyNotFoundException("User not found.");

            if (currentRole == UserRole.Party)
            {
                var partyUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == currentUserId)
                    ?? throw new KeyNotFoundException("Party user not found.");

                if (targetUser.Role != UserRole.Candidate || targetUser.PartyAffiliation != partyUser.FullName)
                {
                    throw new UnauthorizedAccessException("You can only approve candidate accounts affiliated with your party.");
                }
            }
            else if (currentRole == UserRole.ElectionOfficer)
            {
                if (targetUser.Role != UserRole.Voter && targetUser.Role != UserRole.Party && !(targetUser.Role == UserRole.Candidate && targetUser.PartyAffiliation == "Independent"))
                {
                    throw new UnauthorizedAccessException("Election Officers can only approve Voter, Party, and Independent Candidate accounts.");
                }
            }
            else if (currentRole != UserRole.Admin)
            {
                throw new UnauthorizedAccessException("Unauthorized role for approving users.");
            }

            if (currentRole == UserRole.Admin && targetUser.Role == UserRole.Candidate && targetUser.PartyAffiliation != "Independent")
            {
                throw new UnauthorizedAccessException("Candidates affiliated with a party must be approved by their respective party.");
            }

            targetUser.IsApproved = true;
            await dbContext.SaveChangesAsync();
        }

        public async Task RejectUser(Guid currentUserId, UserRole currentRole, Guid targetUserId)
        {
            var targetUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == targetUserId) 
                ?? throw new KeyNotFoundException("User not found.");

            if (currentRole == UserRole.Party)
            {
                var partyUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == currentUserId)
                    ?? throw new KeyNotFoundException("Party user not found.");

                if (targetUser.Role != UserRole.Candidate || targetUser.PartyAffiliation != partyUser.FullName)
                {
                    throw new UnauthorizedAccessException("You can only reject candidate accounts affiliated with your party.");
                }
            }
            else if (currentRole == UserRole.ElectionOfficer)
            {
                if (targetUser.Role != UserRole.Voter && targetUser.Role != UserRole.Party && !(targetUser.Role == UserRole.Candidate && targetUser.PartyAffiliation == "Independent"))
                {
                    throw new UnauthorizedAccessException("Election Officers can only reject Voter, Party, and Independent Candidate accounts.");
                }
            }
            else if (currentRole != UserRole.Admin)
            {
                throw new UnauthorizedAccessException("Unauthorized role for rejecting users.");
            }

            if (currentRole == UserRole.Admin && targetUser.Role == UserRole.Candidate && targetUser.PartyAffiliation != "Independent")
            {
                throw new UnauthorizedAccessException("Candidates affiliated with a party must be rejected by their respective party.");
            }

            dbContext.Users.Remove(targetUser);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<UserResponseDTO>> GetApprovedParties()
        {
            return await dbContext.Users.AsNoTracking()
                .Where(u => u.Role == UserRole.Party && u.IsApproved)
                .Select(u => new UserResponseDTO
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    IsApproved = u.IsApproved,
                    IsVerified = u.IsVerified,
                    PartyAffiliation = u.PartyAffiliation
                }).ToListAsync();
        }

        public async Task<List<UserResponseDTO>> GetParties()
        {
            return await dbContext.Users.AsNoTracking()
                .Where(u => u.Role == UserRole.Party)
                .Select(u => new UserResponseDTO
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    IsApproved = u.IsApproved,
                    IsVerified = u.IsVerified,
                    PartyAffiliation = u.PartyAffiliation
                }).ToListAsync();
        }

        public async Task<List<UserResponseDTO>> GetApprovedCandidates()
        {
            return await dbContext.Users.AsNoTracking()
                .Where(u => u.Role == UserRole.Candidate && u.IsApproved)
                .Select(u => new UserResponseDTO
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    IsApproved = u.IsApproved,
                    IsVerified = u.IsVerified,
                    PartyAffiliation = u.PartyAffiliation
                }).ToListAsync();
        }

        public async Task<List<UserResponseDTO>> GetCandidates()
        {
            return await dbContext.Users.AsNoTracking()
                .Where(u => u.Role == UserRole.Candidate)
                .Select(u => new UserResponseDTO
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    IsApproved = u.IsApproved,
                    IsVerified = u.IsVerified,
                    PartyAffiliation = u.PartyAffiliation
                }).ToListAsync();
        }
    }
}
