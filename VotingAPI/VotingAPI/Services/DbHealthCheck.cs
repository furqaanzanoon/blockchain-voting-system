using Microsoft.Extensions.Diagnostics.HealthChecks;
using VotingAPI.Data;

namespace VotingAPI.Services
{
    public class DbHealthCheck : IHealthCheck
    {
        private readonly VotingDbContext dbContext;

        public DbHealthCheck(VotingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
                if (canConnect)
                {
                    return HealthCheckResult.Healthy("Database connection is healthy.");
                }
                
                return HealthCheckResult.Unhealthy("Cannot connect to the database.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database connection failed.", ex);
            }
        }
    }
}
