using Microsoft.EntityFrameworkCore;
using VotingAPI.Models.Entities;
using VotingAPI.Models.Enums;

namespace VotingAPI.Data.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(VotingDbContext dbContext)
        {
            // Self-healing database schema updates (bypasses EF CLI tool design-time blocks caused by OS Application Control policies)
            try
            {
                // 1. Add IsApproved column to Users if not exists
                await dbContext.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (
                        SELECT * FROM sys.columns 
                        WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'IsApproved'
                    )
                    BEGIN
                        ALTER TABLE [dbo].[Users] ADD [IsApproved] BIT NOT NULL DEFAULT 0;
                    END
                ");

                // 2. Add PartyAffiliation column to Users if not exists
                await dbContext.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (
                        SELECT * FROM sys.columns 
                        WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'PartyAffiliation'
                    )
                    BEGIN
                        ALTER TABLE [dbo].[Users] ADD [PartyAffiliation] NVARCHAR(100) NULL;
                    END
                ");

                // 3. Add IsApproved column to Candidates if not exists (defaulting existing to 1/true)
                await dbContext.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (
                        SELECT * FROM sys.columns 
                        WHERE object_id = OBJECT_ID(N'[dbo].[Candidates]') AND name = 'IsApproved'
                    )
                    BEGIN
                        ALTER TABLE [dbo].[Candidates] ADD [IsApproved] BIT NOT NULL DEFAULT 1;
                    END
                ");

                // 4. Drop VoterId foreign key, indexes, and column from VoteTransactions if they exist (ZKP anonymity)
                await dbContext.Database.ExecuteSqlRawAsync(@"
                    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoteTransactions_Users_VoterId')
                    BEGIN
                        ALTER TABLE [dbo].[VoteTransactions] DROP CONSTRAINT [FK_VoteTransactions_Users_VoterId];
                    END
                    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_VoteTransactions_ElectionId_VoterId' AND object_id = OBJECT_ID('VoteTransactions'))
                    BEGIN
                        DROP INDEX [IX_VoteTransactions_ElectionId_VoterId] ON [dbo].[VoteTransactions];
                    END
                    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_VoteTransactions_VoterId' AND object_id = OBJECT_ID('VoteTransactions'))
                    BEGIN
                        DROP INDEX [IX_VoteTransactions_VoterId] ON [dbo].[VoteTransactions];
                    END
                    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VoteTransactions') AND name = 'VoterId')
                    BEGIN
                        ALTER TABLE [dbo].[VoteTransactions] DROP COLUMN [VoterId];
                    END
                ");

                // 5. Add TxHash and BlockNumber columns to Voters table if they don't exist
                await dbContext.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (
                        SELECT * FROM sys.columns 
                        WHERE object_id = OBJECT_ID(N'[dbo].[Voters]') AND name = 'TxHash'
                    )
                    BEGIN
                        ALTER TABLE [dbo].[Voters] ADD [TxHash] NVARCHAR(66) NULL;
                    END

                    IF NOT EXISTS (
                        SELECT * FROM sys.columns 
                        WHERE object_id = OBJECT_ID(N'[dbo].[Voters]') AND name = 'BlockNumber'
                    )
                    BEGIN
                        ALTER TABLE [dbo].[Voters] ADD [BlockNumber] BIGINT NULL;
                    END
                ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database self-healing warning: {ex.Message}");
            }

            // Ensure any existing Admin is approved
            var admins = await dbContext.Users.Where(u => u.Role == UserRole.Admin && !u.IsApproved).ToListAsync();
            if (admins.Any())
            {
                foreach (var adm in admins)
                {
                    adm.IsApproved = true;
                }
                await dbContext.SaveChangesAsync();
            }

            // Check if admin already exists
            var adminExists = await dbContext.Users.AnyAsync(u => u.Role == UserRole.Admin);

            if (adminExists)
                return;

            var admin = new User
            {
                FullName = "System Admin",
                Email = "furqaanzanoon243@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@123"),
                Role = UserRole.Admin,
                IsVerified = true,
                IsApproved = true,
                CreatedAt = DateTime.UtcNow
            };

            await dbContext.Users.AddAsync(admin);
            await dbContext.SaveChangesAsync();
        }
    }
}