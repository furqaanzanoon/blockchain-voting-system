using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VotingAPI.Services
{
    public class KeepAliveService : BackgroundService
    {
        private readonly ILogger<KeepAliveService> logger;
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;

        public KeepAliveService(ILogger<KeepAliveService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.httpClient = new HttpClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var keepAliveConfig = configuration.GetSection("KeepAlive");
            var enabled = keepAliveConfig.GetValue<bool>("Enabled", false);
            var url = keepAliveConfig.GetValue<string>("Url");
            var intervalMinutes = keepAliveConfig.GetValue<int>("IntervalMinutes", 5);

            if (!enabled || string.IsNullOrEmpty(url))
            {
                logger.LogInformation("Keep-Alive service is disabled or URL is not configured.");
                return;
            }

            logger.LogInformation("Keep-Alive service started. Target URL: {Url}, Interval: {Interval} minutes.", url, intervalMinutes);

            // Wait 10 seconds before the first ping to let the application start up fully
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    logger.LogInformation("Sending keep-alive ping to {Url}...", url);
                    var response = await httpClient.GetAsync(url, stoppingToken);
                    if (response.IsSuccessStatusCode)
                    {
                        logger.LogInformation("Keep-alive ping successful. Status: {StatusCode}", response.StatusCode);
                    }
                    else
                    {
                        logger.LogWarning("Keep-alive ping failed. Status: {StatusCode}", response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred during keep-alive ping to {Url}.", url);
                }

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        public override void Dispose()
        {
            httpClient.Dispose();
            base.Dispose();
        }
    }
}
