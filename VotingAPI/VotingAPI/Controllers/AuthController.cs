using Microsoft.AspNetCore.Mvc;
using VotingAPI.Models.DTOs.Auth;
using VotingAPI.Services.Interfaces;

namespace VotingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IWebHostEnvironment env;

        public AuthController(IAuthService authService, IWebHostEnvironment env)
        {
            this.authService = authService;
            this.env = env;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        {
            var result = await authService.Register(registerRequestDTO);
            return Ok(new { message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var message = await authService.Login(loginRequestDTO);

            return Ok(new { requiresOtp = true, message, email = loginRequestDTO.Email });
        }

        [HttpPost("verify-login-otp")]
        public async Task<IActionResult> VerifyLoginOtp([FromBody] VerifyOtpDTO verifyOtpDTO)
        {
            var token = await authService.VerifyLoginOtp(verifyOtpDTO);

            Response.Cookies.Append(key: "access_token", value: token, options: new CookieOptions
            {
                HttpOnly = true,
                Secure = !env.IsDevelopment(), // Secure=true in production (HTTPS only)
                SameSite = env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(1)
            });

            return Ok(token);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDTO verifyOtpDTO)
        {
            var result = await authService.VerifyOtp(verifyOtpDTO);
            return Ok(new { message = result });
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDTO resendOtpDTO)
        {
            var result = await authService.ResendOtp(resendOtpDTO);
            return Ok(new { message = result });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO forgotPasswordRequestDTO)
        {
            var result = await authService.ForgotPassword(forgotPasswordRequestDTO);
            return Ok(new { message = result });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO resetPasswordRequestDTO)
        {
            var result = await authService.ResetPassword(resetPasswordRequestDTO);
            return Ok(new { message = result });
        }
    }
}