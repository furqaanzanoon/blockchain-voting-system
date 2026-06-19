using System.Text.Json;

namespace VotingAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unhandled exception occurred during request processing");

                context.Response.ContentType = "application/json";

                var statusCode = ex switch
                {
                    ArgumentException => 400,
                    UnauthorizedAccessException => 401,
                    KeyNotFoundException => 404,
                    InvalidOperationException => 400,    // Business rule violations (e.g. "Already voted", "Election not active")
                    NotImplementedException => 501,
                    _ => 500,
                };

                context.Response.StatusCode = statusCode;

                var message = statusCode == 500 
                    ? "An unexpected error occurred. Please try again later." 
                    : (ex.InnerException?.Message ?? ex.Message);

                var response = new
                {
                    message = message
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response)
                );
            }
        }
    }
}