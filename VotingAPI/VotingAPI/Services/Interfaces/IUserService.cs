using VotingAPI.Models.DTOs.User;
using VotingAPI.Models.Enums;

namespace VotingAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserResponseDTO>> GetAllUsers();
        Task<string> ConnectWallet(Guid userId, string ethAddress);
        Task<string> ChangePassword(Guid userId, ChangePasswordRequestDTO changePasswordRequestDTO);
        Task<List<UserResponseDTO>> GetPendingUsers(Guid currentUserId, UserRole currentRole);
        Task ApproveUser(Guid currentUserId, UserRole currentRole, Guid targetUserId);
        Task RejectUser(Guid currentUserId, UserRole currentRole, Guid targetUserId);
        Task<List<UserResponseDTO>> GetApprovedParties();
        Task<List<UserResponseDTO>> GetParties();
        Task<List<UserResponseDTO>> GetApprovedCandidates();
        Task<List<UserResponseDTO>> GetCandidates();
    }
}