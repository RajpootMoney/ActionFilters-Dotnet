using System.Diagnostics;

namespace ActionFiltersAPI.Middleware
{
    /// <summary>
    /// CUSTOM MIDDLEWARE EXAMPLE - Performance Monitoring
    /// 
    /// This middleware demonstrates:
    /// - Short-circuiting the pipeline (returning early)
    /// - Adding custom headers to responses
    /// - Performance monitoring
    /// 
    /// Middleware Order Matters:
    /// - This should be registered early to catch all requests
    /// - But after exception handling middleware
    /// </summary>
    public class PerformanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMiddleware> _logger;
        private const long SlowRequestThresholdMs = 1000; // 1 second

        public PerformanceMiddleware(
            RequestDelegate next,
            ILogger<PerformanceMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Example: Short-circuit for health check endpoints (optional)
            // if (context.Request.Path.StartsWithSegments("/health"))
            // {
            //     await context.Response.WriteAsync("OK");
            //     return; // Don't call _next, pipeline stops here
            // }

            // Call next middleware
            await _next(context);

            // After response is generated
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            // Add custom header with execution time
            context.Response.Headers.Append("X-Execution-Time-Ms", elapsedMs.ToString());

            // Log slow requests
            if (elapsedMs > SlowRequestThresholdMs)
            {
                _logger.LogWarning(
                    "PERFORMANCE MIDDLEWARE - Slow request detected. " +
                    "Path: {Path}, Method: {Method}, ExecutionTime: {Time}ms",
                    context.Request.Path,
                    context.Request.Method,
                    elapsedMs
                );
            }
        }
    }

    public static class PerformanceMiddlewareExtensions
    {
        public static IApplicationBuilder UsePerformanceMonitoring(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PerformanceMiddleware>();
        }
    }
}

