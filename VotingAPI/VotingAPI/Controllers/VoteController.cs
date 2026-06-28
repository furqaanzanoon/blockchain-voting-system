using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VotingAPI.Models.DTOs.Vote;
using VotingAPI.Models.Enums;
using VotingAPI.Services.Interfaces;

namespace VotingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VoteController : ControllerBase
    {
        private readonly IVoteService voteService;

        public VoteController(IVoteService voteService)
        {
            this.voteService = voteService;
        }

        [Authorize(Roles = nameof(UserRole.Voter))]
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendVoteOtp([FromBody] VotePrepareRequestDTO votePrepareRequestDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Not logged in");

            var result = await voteService.SendVoteOtp(Guid.Parse(userId!), votePrepareRequestDTO);
            return Ok(new { message = result });
        }

        [Authorize(Roles = nameof(UserRole.Voter))]
        [HttpGet("nonce/{electionId:guid}")]
        public async Task<IActionResult> GetVoterNonce(Guid electionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Not logged in");

            var (nonce, registeredAddress) = await voteService.GetVoterNonce(Guid.Parse(userId!), electionId);
            return Ok(new { nonce = nonce, registeredAddress = registeredAddress });
        }

        [Authorize(Roles = nameof(UserRole.Voter))]
        [HttpPost("prepare")]
        public async Task<IActionResult> PrepareVote([FromBody] VotePrepareRequestDTO votePrepareRequestDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Not logged in");

            var result = await voteService.PrepareVote(Guid.Parse(userId!), votePrepareRequestDTO);
            return Ok(new { message = result });
        }

    }
}
