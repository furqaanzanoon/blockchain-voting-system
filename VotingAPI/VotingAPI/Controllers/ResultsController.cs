using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingAPI.Models.Enums;
using VotingAPI.Services.Interfaces;

namespace VotingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ResultsController : ControllerBase
    {
        private readonly IResultService resultService;

        public ResultsController(IResultService resultService)
        {
            this.resultService = resultService;
        }

        [HttpGet("{electionId:guid}")]
        public async Task<IActionResult> GetResults(Guid electionId)
        {
            var results = await resultService.GetElectionResults(electionId);
            return Ok(results);
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpGet("{electionId:guid}/audit")]
        public async Task<IActionResult> GetAudit(Guid electionId)
        {
            var result = await resultService.GetElectionAudit(electionId);
            return Ok(new { message = result });
        }
    }
}