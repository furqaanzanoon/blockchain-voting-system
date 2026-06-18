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
    [Authorize(Roles = nameof(UserRole.Voter))]
    public class VoteController : ControllerBase
    {
        private readonly IVoteService voteService;

        public VoteController(IVoteService voteService)
        {
            this.voteService = voteService;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendVoteOtp([FromBody] VotePrepareRequestDTO votePrepareRequestDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Not logged in");

            var result = await voteService.SendVoteOtp(Guid.Parse(userId!), votePrepareRequestDTO);
            return Ok(new { message = result });
        }

        [HttpGet("nonce/{electionId:guid}")]
        public async Task<IActionResult> GetVoterNonce(Guid electionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Not logged in");

            var (nonce, registeredAddress) = await voteService.GetVoterNonce(Guid.Parse(userId!), electionId);
            return Ok(new { nonce = nonce, registeredAddress = registeredAddress });
        }

        [HttpPost("prepare")]
        public async Task<IActionResult> PrepareVote([FromBody] VotePrepareRequestDTO votePrepareRequestDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Not logged in");

            var result = await voteService.PrepareVote(Guid.Parse(userId!), votePrepareRequestDTO);
            return Ok(new { message = result });
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmVote([FromBody] ConfirmVoteDTO confirmVoteDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Not logged in");

            await voteService.ConfirmVote(Guid.Parse(userId!), confirmVoteDTO);
            return Ok(new { message = "Vote stored successfully" });
        }

        [HttpGet("receipt/{electionId:guid}")]
        public async Task<IActionResult> GetVoteReceipt(Guid electionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Not logged in");

            var receipt = await voteService.GetVoteReceipt(Guid.Parse(userId!), electionId);
            if (receipt == null)
                return NotFound(new { message = "No vote found for this election" });

            return Ok(receipt);
        }

        [HttpPost("zk")]
        public async Task<IActionResult> CastZkVote([FromBody] CastZkVoteDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Not logged in");

            var (txHash, blockNumber) = await voteService.CastZkVoteAsync(Guid.Parse(userId!), dto);
            return Ok(new { txHash = txHash, blockNumber = blockNumber });
        }
    }
}
