using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingAPI.Models.DTOs.Voter;
using VotingAPI.Models.Enums;
using VotingAPI.Services.Interfaces;

namespace VotingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.ElectionOfficer)}")]
    public class VotersController : ControllerBase
    {
        private readonly IVoterService voterService;

        public VotersController(IVoterService voterService)
        {
            this.voterService = voterService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Guid ElectionId, Guid UserId)
        {
            var result = await voterService.RegisterVoter(ElectionId, UserId);
            return Ok(new { message = result });
        }

        [HttpGet("{electionId:guid}")]
        public async Task<IActionResult> GetVoters(Guid electionId)
        {
            var result = await voterService.GetElectionVoters(electionId);
            return Ok(result);
        }
    }
}