using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;
using VotingAPI.Data;
using VotingAPI.Models.DTOs.Voter;
using VotingAPI.Models.Entities;
using VotingAPI.Models.Enums;
using VotingAPI.Services.Interfaces;

namespace VotingAPI.Services
{
    public class VoterService : IVoterService
    {
        private readonly VotingDbContext dbContext;
        private readonly IBlockchainService blockchainService;

        public VoterService(VotingDbContext dbContext, IBlockchainService blockchainService)
        {
            this.dbContext = dbContext;
            this.blockchainService = blockchainService;
        }

        public async Task<string> RegisterVoter(Guid ElectionId, Guid UserId)
        {
            var election = await dbContext.Elections.FirstOrDefaultAsync(e => e.ElectionId == ElectionId) ?? throw new KeyNotFoundException("Election not found");

            if (election.Status != ElectionStatus.Draft)
                throw new ArgumentException("Voter can be registered only in draft election");

            if (election.StartTime <= DateTime.UtcNow)
                throw new ArgumentException("Election has already been started");

            if (election.EndTime <= DateTime.UtcNow)
                throw new ArgumentException("Election has already been ended");

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == UserId) ?? throw new KeyNotFoundException("Voter not found");

            if (user.Role != UserRole.Voter)
                throw new ArgumentException("Only voter is eligible to vote");

            if (string.IsNullOrWhiteSpace(user.EthAddress))
                throw new ArgumentException("No wallet connected");

            var voterExists = await dbContext.Voters.AnyAsync(v => v.UserId == UserId && v.ElectionId == ElectionId);

            if (voterExists)
                throw new InvalidOperationException("Voter already exists");

            var voter = new Voter
            {
                HasVoted = false,
                RegisteredAt = DateTime.UtcNow,
                ElectionId = ElectionId,
                UserId = UserId
            };

            await dbContext.Voters.AddAsync(voter);
            await dbContext.SaveChangesAsync();

            return "Voter registered to this election successfully";
        }

        public async Task<List<VoterResponseDTO>> GetElectionVoters(Guid electionId)
        {
            var electionExists = await dbContext.Elections.AnyAsync(e => e.ElectionId == electionId);

            if (!electionExists)
                throw new KeyNotFoundException("Election not found");

            var voters = await dbContext.Voters.Where(v => v.ElectionId == electionId).Include(v => v.User).OrderBy(v => v.RegisteredAt).Select(v => new VoterResponseDTO
            {
                UserId = v.User.UserId,
                FullName = v.User.FullName,
                Email = v.User.Email,
                EthAddress = v.User.EthAddress,
                HasVoted = v.HasVoted,
                RegisteredAt = v.RegisteredAt
            }).ToListAsync();

            return voters;
        }
    }
}