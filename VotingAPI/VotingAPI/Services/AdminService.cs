using Microsoft.EntityFrameworkCore;
using VotingAPI.Data;
using VotingAPI.Helpers;
using VotingAPI.Models.DTOs.Admin;
using VotingAPI.Models.DTOs.User;
using VotingAPI.Models.Entities;
using VotingAPI.Models.Enums;
using VotingAPI.Services.Interfaces;

namespace VotingAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly VotingDbContext dbContext;
        private readonly IEmailService emailService;

        public AdminService(VotingDbContext dbContext, IEmailService emailService)
        {
            this.dbContext = dbContext;
            this.emailService = emailService;
        }

        public async Task<string> CreateOfficer(CreateOfficerDTO dto)
        {
            var officerExists = await dbContext.Users.AnyAsync(u => u.Email == dto.Email);

            if (officerExists)
                throw new InvalidOperationException("User already exists");

            var otp = EmailHelper.GetOtp();
            var body = EmailHelper.GetBody(dto.FullName, otp);

            var officer = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.ElectionOfficer,
                IsVerified = false,
                IsApproved = true,
                OtpCode = otp,
                OtpExpiry = DateTime.UtcNow.AddMinutes(10),
                CreatedAt = DateTime.UtcNow
            };

            await dbContext.Users.AddAsync(officer);
            await dbContext.SaveChangesAsync();

            await emailService.SendEmailAsync(officer.Email, subject: "OTP Verification", body: body);

            return "Registration successful. OTP sent to email.";
        }

        public async Task<List<UserResponseDTO>> GetElectionOfficers()
        {
            return await dbContext.Users.AsNoTracking()
                .Where(u => u.Role == UserRole.ElectionOfficer)
                .Select(u => new UserResponseDTO
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    IsApproved = u.IsApproved,
                    IsVerified = u.IsVerified
                }).ToListAsync();
        }
    }
}
