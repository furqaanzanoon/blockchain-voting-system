using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VotingAPI.Models.DTOs.Election;
using VotingAPI.Models.Enums;
using VotingAPI.Services;
using VotingAPI.Services.Interfaces;

namespace VotingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ElectionsController : ControllerBase
    {
        private readonly IElectionService electionService;

        public ElectionsController(IElectionService electionService)
        {
            this.electionService = electionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllElections()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? currentUserId = userIdClaim != null ? Guid.Parse(userIdClaim) : null;

            var elections = await electionService.GetAllElections(currentUserId);
            return Ok(new { elections });
        }

        [HttpGet("{electionId:guid}")]
        public async Task<IActionResult> GetElectionById(Guid electionId)
        {
            var election = await electionService.GetElectionById(electionId);
            return Ok(new { election });
        }

        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.ElectionOfficer)}")]
        [HttpPost]
        public async Task<IActionResult> CreateElection([FromBody] CreateElectionDTO createElectionDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User not found");

            var result = await electionService.CreateElection(createElectionDTO, Guid.Parse(userId));
            return Ok(new { message = result });
        }

        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.ElectionOfficer)}")]
        [HttpPut("{electionId:guid}/activate")]
        public async Task<IActionResult> ActivateElection(Guid electionId)
        {
            var result = await electionService.ActivateElection(electionId);
            return Ok(new { message = result });
        }

        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.ElectionOfficer)}")]
        [HttpPut("{electionId:guid}/close")]
        public async Task<IActionResult> CloseElection(Guid electionId)
        {
            var result = await electionService.CloseElection(electionId);
            return Ok(new { message = result });
        }

        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.ElectionOfficer)},{nameof(UserRole.Party)},{nameof(UserRole.Candidate)}")]
        [HttpPost("{electionId:guid}/candidates")]
        public async Task<IActionResult> AddCandidate(Guid electionId, [FromBody] AddCandidateDTO addCandidateDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User not found");
            var roleStr = User.FindFirstValue(ClaimTypes.Role) ?? throw new UnauthorizedAccessException("Role claim is missing");
            var role = Enum.Parse<UserRole>(roleStr);
            var fullName = User.FindFirstValue(ClaimTypes.Name) ?? "";

            var result = await electionService.AddCandidate(electionId, addCandidateDTO, role, fullName);
            return Ok(new { message = result });
        }

        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.ElectionOfficer)}")]
        [HttpPost("{electionId:guid}/candidates/{candidateId:guid}/approve")]
        public async Task<IActionResult> ApproveCandidate(Guid electionId, Guid candidateId)
        {
            var result = await electionService.ApproveCandidate(electionId, candidateId);
            return Ok(new { message = result });
        }

        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.ElectionOfficer)}")]
        [HttpDelete("{electionId:guid}/candidates/{candidateId:guid}")]
        public async Task<IActionResult> RemoveCandidate(Guid electionId, Guid candidateId)
        {
            var result = await electionService.RemoveCandidate(electionId, candidateId);
            return Ok(new { message = result });
        }

        [HttpGet("{electionId:guid}/candidates")]
        public async Task<IActionResult> GetCandidates(Guid electionId)
        {
            var roleClaim = User.FindFirstValue(ClaimTypes.Role);
            bool includeUnapproved = false;
            if (roleClaim != null)
            {
                if (Enum.TryParse<UserRole>(roleClaim, out var role))
                {
                    if (role == UserRole.Admin || role == UserRole.ElectionOfficer || role == UserRole.Party || role == UserRole.Candidate)
                    {
                        includeUnapproved = true;
                    }
                }
            }

            var result = await electionService.GetCandidates(electionId, includeUnapproved);
            return Ok(result);
        }
    }
}