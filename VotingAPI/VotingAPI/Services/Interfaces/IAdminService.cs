using VotingAPI.Models.DTOs.Admin;
using VotingAPI.Models.DTOs.User;

namespace VotingAPI.Services.Interfaces
{
    public interface IAdminService
    {
        Task<string> CreateOfficer(CreateOfficerDTO dto);
        Task<List<UserResponseDTO>> GetElectionOfficers();
    }
}