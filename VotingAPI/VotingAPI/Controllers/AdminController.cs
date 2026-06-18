using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingAPI.Models.DTOs.Admin;
using VotingAPI.Models.Enums;
using VotingAPI.Services.Interfaces;

namespace VotingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService adminService;

        public AdminController(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        [HttpPost("create-officer")]
        public async Task<IActionResult> CreateOfficer(CreateOfficerDTO dto)
        {
            var result = await adminService.CreateOfficer(dto);
            return Ok(new { messsage = result });
        }

        [HttpGet("officers")]
        public async Task<IActionResult> GetElectionOfficers()
        {
            var result = await adminService.GetElectionOfficers();
            return Ok(result);
        }
    }
}