using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VotingAPI.Data;
using VotingAPI.Helpers;
using VotingAPI.Models.DTOs.Auth;
using VotingAPI.Models.Entities;
using VotingAPI.Models.Enums;
using VotingAPI.Services.Interfaces;

namespace VotingAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly VotingDbContext dbContext;
        private readonly IJwtService jwtService;
        private readonly IEmailService emailService;
        private readonly IMemoryCache memoryCache;

        public class PendingRegistration
        {
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PasswordHash { get; set; } = string.Empty;
            public UserRole Role { get; set; }
            public string? PartyAffiliation { get; set; }
            public string OtpCode { get; set; } = string.Empty;
            public DateTime OtpExpiry { get; set; }
        }

        public AuthService(VotingDbContext dbContext, IJwtService jwtService, IEmailService emailService, IMemoryCache memoryCache)
        {
            this.dbContext = dbContext;
            this.jwtService = jwtService;
            this.emailService = emailService;
            this.memoryCache = memoryCache;
        }

        public async Task<string> Register(RegisterRequestDTO registerRequest)
        {
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(user => user.Email == registerRequest.Email);

            if (existingUser != null)
                throw new ArgumentException("User with this email already exists.");

            var otp = EmailHelper.GetOtp();
            var body = EmailHelper.GetBody(registerRequest.FullName, otp);

            var pending = new PendingRegistration
            {
                FullName = registerRequest.FullName,
                Email = registerRequest.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password),
                Role = registerRequest.Role,
                PartyAffiliation = registerRequest.PartyAffiliation,
                OtpCode = otp,
                OtpExpiry = DateTime.UtcNow.AddMinutes(10)
            };

            var cacheKey = $"reg_{registerRequest.Email.ToLowerInvariant()}";
            memoryCache.Set(cacheKey, pending, TimeSpan.FromMinutes(10));

            await emailService.SendEmailAsync(toEmail: pending.Email, subject: "OTP Verification", body: body);

            return "Registration successful. OTP sent to email.";
        }

        public async Task<string> VerifyOtp(VerifyOtpDTO verifyOtpDTO)
        {
            var cacheKey = $"reg_{verifyOtpDTO.Email.ToLowerInvariant()}";
            if (!memoryCache.TryGetValue(cacheKey, out PendingRegistration? pending) || pending == null)
            {
                var userExists = await dbContext.Users.AnyAsync(u => u.Email == verifyOtpDTO.Email);
                if (userExists)
                    throw new ArgumentException("User already verified.");

                throw new KeyNotFoundException("Registration expired or not found. Please register again.");
            }

            if (pending.OtpCode != verifyOtpDTO.Otp)
                throw new InvalidOperationException("Invalid OTP.");

            if (pending.OtpExpiry < DateTime.UtcNow)
                throw new InvalidOperationException("OTP has expired.");

            // Concurrency check before adding user
            var userCollision = await dbContext.Users.AnyAsync(u => u.Email == pending.Email);
            if (userCollision)
                throw new ArgumentException("User with this email already exists.");

            User user = new()
            {
                FullName = pending.FullName,
                Email = pending.Email,
                PasswordHash = pending.PasswordHash,
                EthAddress = null,
                Role = pending.Role,
                PartyAffiliation = pending.PartyAffiliation,
                IsVerified = true,
                IsApproved = false,
                OtpCode = null,
                OtpExpiry = null,
                CreatedAt = DateTime.UtcNow
            };

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            memoryCache.Remove(cacheKey);

            return "Account verified successfully.";
        }

        public async Task<string> ResendOtp(ResendOtpDTO resendOtpDTO)
        {
            var cacheKey = $"reg_{resendOtpDTO.Email.ToLowerInvariant()}";
            if (!memoryCache.TryGetValue(cacheKey, out PendingRegistration? pending) || pending == null)
            {
                var userExists = await dbContext.Users.AnyAsync(u => u.Email == resendOtpDTO.Email);
                if (userExists)
                    throw new ArgumentException("User already verified.");

                throw new KeyNotFoundException("Registration expired or not found. Please register again.");
            }

            if (pending.OtpExpiry > DateTime.UtcNow)
                throw new ArgumentException("Current OTP is still valid. Please wait before requesting a new one.");

            var otp = EmailHelper.GetOtp();
            var body = EmailHelper.GetBody(pending.FullName, otp);

            pending.OtpCode = otp;
            pending.OtpExpiry = DateTime.UtcNow.AddMinutes(10);

            memoryCache.Set(cacheKey, pending, TimeSpan.FromMinutes(10));

            await emailService.SendEmailAsync(toEmail: pending.Email, subject: "OTP Verification", body: body);

            return "OTP resent successfully.";
        }

        public async Task<string> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == loginRequestDTO.Email) ?? throw new KeyNotFoundException("User not found");

            if (!BCrypt.Net.BCrypt.Verify(loginRequestDTO.Password, user.PasswordHash))
                throw new InvalidOperationException("Invalid credentials.");

            if (!user.IsVerified)
                throw new UnauthorizedAccessException("Account not verified. Please verify your account before logging in.");

            if (!user.IsApproved)
            {
                if (user.Role == UserRole.Candidate && user.PartyAffiliation != "Independent")
                {
                    throw new UnauthorizedAccessException($"Your account is pending approval by your affiliated party ({user.PartyAffiliation}).");
                }
                throw new UnauthorizedAccessException("Your account is pending approval by an administrator or election officer.");
            }

            if (loginRequestDTO.Role != user.Role)
                throw new InvalidOperationException("Invalid role.");

            var otp = EmailHelper.GetOtp();
            var body = EmailHelper.GetBody(user.FullName, otp, validMinutes: 5);

            user.OtpCode = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(5);

            await dbContext.SaveChangesAsync();

            await emailService.SendEmailAsync(toEmail: user.Email, subject: "Login OTP Verification", body: body);

            return "OTP sent to your email.";
        }

        public async Task<string> VerifyLoginOtp(VerifyOtpDTO verifyOtpDTO)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == verifyOtpDTO.Email) ?? throw new KeyNotFoundException("User not found.");

            if (user.OtpCode != verifyOtpDTO.Otp)
                throw new InvalidOperationException("Invalid OTP.");

            if (user.OtpExpiry == null || user.OtpExpiry < DateTime.UtcNow)
                throw new InvalidOperationException("OTP has expired.");

            user.OtpCode = null;
            user.OtpExpiry = null;

            await dbContext.SaveChangesAsync();

            var token = jwtService.GenerateToken(user);

            return token;
        }

        public async Task<string> ForgotPassword(ForgotPasswordRequestDTO forgotPasswordRequestDTO)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == forgotPasswordRequestDTO.Email) ?? throw new KeyNotFoundException("User with this email does not exist.");

            var otp = EmailHelper.GetOtp();
            var body = EmailHelper.GetBody(user.FullName, otp);

            user.OtpCode = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);

            await dbContext.SaveChangesAsync();

            await emailService.SendEmailAsync(toEmail: user.Email, subject: "Password Reset OTP", body: body);

            return "Password reset OTP sent to your email.";
        }

        public async Task<string> ResetPassword(ResetPasswordRequestDTO resetPasswordRequestDTO)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == resetPasswordRequestDTO.Email) ?? throw new KeyNotFoundException("User not found.");

            if (user.OtpCode != resetPasswordRequestDTO.Otp)
                throw new InvalidOperationException("Invalid OTP.");

            if (user.OtpExpiry == null || user.OtpExpiry < DateTime.UtcNow)
                throw new InvalidOperationException("OTP has expired.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordRequestDTO.NewPassword);
            user.OtpCode = null;
            user.OtpExpiry = null;

            await dbContext.SaveChangesAsync();

            return "Password reset successfully. You can now login with your new password.";
        }
    }
}