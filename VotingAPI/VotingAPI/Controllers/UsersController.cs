using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VotingAPI.Models.DTOs.User;
using VotingAPI.Models.Enums;
using VotingAPI.Services.Interfaces;

namespace VotingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.ElectionOfficer)},{nameof(UserRole.Party)},{nameof(UserRole.Candidate)}")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await userService.GetAllUsers();
            return Ok(new { message = result });
        }

        [Authorize(Roles = nameof(UserRole.Voter))]
        [HttpPost("connect-wallet")]
        public async Task<IActionResult> ConnectWallet(string ethAddress)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User not logged-in");

            var result = await userService.ConnectWallet(Guid.Parse(userId), ethAddress);
            return Ok(new { message = result });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO changePasswordRequestDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User not logged-in");

            var result = await userService.ChangePassword(Guid.Parse(userId), changePasswordRequestDTO);
            return Ok(new { message = result });
        }

        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.ElectionOfficer)},{nameof(UserRole.Party)}")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User not logged-in");
            var roleStr = User.FindFirstValue(ClaimTypes.Role) ?? throw new UnauthorizedAccessException("Role claim is missing");
            var role = Enum.Parse<UserRole>(roleStr);

            var result = await userService.GetPendingUsers(Guid.Parse(userId), role);
            return Ok(result);
        }

        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.ElectionOfficer)},{nameof(UserRole.Party)}")]
        [HttpPost("{userId}/approve")]
        public async Task<IActionResult> ApproveUser(Guid userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User not logged-in");
            var roleStr = User.FindFirstValue(ClaimTypes.Role) ?? throw new UnauthorizedAccessException("Role claim is missing");
            var role = Enum.Parse<UserRole>(roleStr);

            await userService.ApproveUser(Guid.Parse(currentUserId), role, userId);
            return Ok(new { message = "User approved successfully." });
        }

        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.ElectionOfficer)},{nameof(UserRole.Party)}")]
        [HttpPost("{userId}/reject")]
        public async Task<IActionResult> RejectUser(Guid userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User not logged-in");
            var roleStr = User.FindFirstValue(ClaimTypes.Role) ?? throw new UnauthorizedAccessException("Role claim is missing");
            var role = Enum.Parse<UserRole>(roleStr);

            await userService.RejectUser(Guid.Parse(currentUserId), role, userId);
            return Ok(new { message = "User rejected successfully." });
        }

        [AllowAnonymous]
        [HttpGet("parties")]
        public async Task<IActionResult> GetApprovedParties()
        {
            var result = await userService.GetApprovedParties();
            return Ok(result);
        }

        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.ElectionOfficer)}")]
        [HttpGet("all-parties")]
        public async Task<IActionResult> GetParties()
        {
            var result = await userService.GetParties();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("candidates")]
        public async Task<IActionResult> GetApprovedCandidates()
        {
            var result = await userService.GetApprovedCandidates();
            return Ok(result);
        }

        [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.ElectionOfficer)}")]
        [HttpGet("all-candidates")]
        public async Task<IActionResult> GetCandidates()
        {
            var result = await userService.GetCandidates();
            return Ok(result);
        }
    }
}
