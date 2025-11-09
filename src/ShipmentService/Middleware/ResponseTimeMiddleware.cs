using System.Diagnostics;

namespace ShipmentService.Middleware
{
    public class ResponseTimeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResponseTimeMiddleware> _logger;

        public ResponseTimeMiddleware(RequestDelegate next, ILogger<ResponseTimeMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Apply middleware only for API endpoints
            if (!context.Request.Path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();

            // Hook before response starts
            context.Response.OnStarting(() =>
            {
                stopwatch.Stop();
                var elapsedMs = stopwatch.ElapsedMilliseconds;

                context.Response.Headers["X-Response-Time"] = $"{elapsedMs}ms";

                _logger.LogInformation(
                    "[{Method}] {Path} took {Elapsed}ms",
                    context.Request.Method,
                    context.Request.Path,
                    elapsedMs
                );

                return Task.CompletedTask;
            });

            await _next(context);
        }
    }

    // Extension method for cleaner registration
    public static class ResponseTimeMiddlewareExtensions
    {
        public static IApplicationBuilder UseResponseTimeMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseTimeMiddleware>();
        }
    }
}
