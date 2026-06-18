using Microsoft.EntityFrameworkCore;
using VotingAPI.Data;
using VotingAPI.Models.Enums;
using VotingAPI.Services.Interfaces;

namespace VotingAPI.Services
{
    public class ElectionAutoActivateService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<ElectionAutoActivateService> logger;

        public ElectionAutoActivateService(IServiceProvider serviceProvider, ILogger<ElectionAutoActivateService> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<VotingDbContext>();
                    var electionService = scope.ServiceProvider.GetRequiredService<IElectionService>();

                    await ProcessAutoActivations(dbContext, electionService, stoppingToken);
                    await ProcessAutoClosures(dbContext, electionService, stoppingToken);
                    await CleanupExpiredDrafts(dbContext, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in election background service");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private async Task ProcessAutoActivations(VotingDbContext dbContext, IElectionService electionService, CancellationToken stoppingToken)
        {
            var eligibleElections = await dbContext.Elections
                .Where(e => e.AutoActivate && e.Status == ElectionStatus.Draft && e.StartTime <= DateTime.UtcNow && e.EndTime > DateTime.UtcNow)
                .ToListAsync(stoppingToken);

            foreach (var election in eligibleElections)
            {
                try
                {
                    await electionService.ActivateElection(election.ElectionId);
                    logger.LogInformation("Auto-activated election: {Title} ({Id})", election.Title, election.ElectionId);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to auto-activate election: {Title} ({Id}) - {Message}", election.Title, election.ElectionId, ex.Message);

                    // Mark auto-activate as failed so frontend can show a warning
                    election.AutoActivate = false;
                    election.AutoActivateFailReason = ex.Message;
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
        }

        private async Task ProcessAutoClosures(VotingDbContext dbContext, IElectionService electionService, CancellationToken stoppingToken)
        {
            var eligibleElections = await dbContext.Elections
                .Where(e => e.AutoClose && e.Status == ElectionStatus.Active && e.EndTime <= DateTime.UtcNow)
                .ToListAsync(stoppingToken);

            foreach (var election in eligibleElections)
            {
                try
                {
                    await electionService.CloseElection(election.ElectionId);
                    logger.LogInformation("Auto-closed election: {Title} ({Id})", election.Title, election.ElectionId);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to auto-close election: {Title} ({Id}) - {Message}", election.Title, election.ElectionId, ex.Message);
                    
                    // Mark auto-close as disabled to prevent continuous retries on fail
                    election.AutoClose = false;
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
        }

        private async Task CleanupExpiredDrafts(VotingDbContext dbContext, CancellationToken stoppingToken)
        {
            var expiredDrafts = await dbContext.Elections
                .Include(e => e.Candidates)
                .Include(e => e.Voters)
                .Where(e => e.Status == ElectionStatus.Draft && e.EndTime < DateTime.UtcNow)
                .ToListAsync(stoppingToken);

            if (expiredDrafts.Count == 0)
                return;

            foreach (var election in expiredDrafts)
            {
                dbContext.Candidates.RemoveRange(election.Candidates);
                dbContext.Voters.RemoveRange(election.Voters);
                dbContext.Elections.Remove(election);

                logger.LogInformation("Deleted expired draft election: {Title} ({Id})", election.Title, election.ElectionId);
            }

            await dbContext.SaveChangesAsync(stoppingToken);
        }
    }
}

