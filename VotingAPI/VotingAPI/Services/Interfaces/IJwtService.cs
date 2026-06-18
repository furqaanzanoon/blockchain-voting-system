using VotingAPI.Models.Entities;

namespace VotingAPI.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}