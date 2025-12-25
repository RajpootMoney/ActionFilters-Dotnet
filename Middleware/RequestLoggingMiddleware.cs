using System.Diagnostics;

namespace ActionFiltersAPI.Middleware
{
    /// <summary>
    /// CUSTOM MIDDLEWARE EXAMPLE
    /// 
    /// Middleware components form the Request Pipeline in ASP.NET Core.
    /// They are executed in the order they are registered in Program.cs.
    /// 
    /// Request Pipeline Flow:
    /// 1. HTTP Request arrives
    /// 2. Middleware 1 (first registered)
    /// 3. Middleware 2
    /// 4. ... (more middleware)
    /// 5. Routing Middleware (maps request to controller/action)
    /// 6. Authorization Middleware
    /// 7. Endpoint Middleware (executes controller/action)
    /// 8. Action Filters (if applied)
    /// 9. Exception Filters (if exception occurs)
    /// 10. Middleware 2 (response phase - reverse order)
    /// 11. Middleware 1 (response phase)
    /// 12. HTTP Response sent
    /// 
    /// Middleware can:
    /// - Process requests before they reach controllers
    /// - Process responses after controllers execute
    /// - Short-circuit the pipeline (return response early)
    /// - Pass control to next middleware
    /// 
    /// This middleware logs all incoming requests and their execution time.
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next; // Reference to next middleware in pipeline
            _logger = logger;
        }

        /// <summary>
        /// InvokeAsync is called for each HTTP request
        /// This is where middleware logic executes
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            // PHASE 1: REQUEST PROCESSING (before controller)
            var stopwatch = Stopwatch.StartNew();
            var requestPath = context.Request.Path;
            var requestMethod = context.Request.Method;
            var requestQuery = context.Request.QueryString.ToString();
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            _logger.LogInformation(
                "MIDDLEWARE - Request received. " +
                "Method: {Method}, Path: {Path}, Query: {Query}, IP: {IP}, " +
                "Timestamp: {Timestamp}",
                requestMethod,
                requestPath,
                requestQuery,
                clientIp,
                DateTime.UtcNow
            );

            // Store original response body stream
            var originalBodyStream = context.Response.Body;

            try
            {
                // Create a new memory stream to capture response
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                // Call next middleware in pipeline
                // This passes control to the next component (could be another middleware or the endpoint)
                // Execution will return here after all subsequent middleware and controllers complete
                await _next(context);

                // PHASE 2: RESPONSE PROCESSING (after controller)
                stopwatch.Stop();
                var responseStatusCode = context.Response.StatusCode;
                var executionTime = stopwatch.ElapsedMilliseconds;

                // Read response body
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();
                responseBody.Seek(0, SeekOrigin.Begin);

                // Copy response back to original stream
                await responseBody.CopyToAsync(originalBodyStream);

                _logger.LogInformation(
                    "MIDDLEWARE - Request completed. " +
                    "Method: {Method}, Path: {Path}, " +
                    "Status: {Status}, ExecutionTime: {Time}ms, " +
                    "ResponseLength: {Length} bytes",
                    requestMethod,
                    requestPath,
                    responseStatusCode,
                    executionTime,
                    responseBodyText.Length
                );
            }
            catch (Exception ex)
            {
                // If exception occurs, log it
                // Note: Exception Filters will also catch this if not handled
                _logger.LogError(
                    ex,
                    "MIDDLEWARE - Exception occurred during request processing. " +
                    "Path: {Path}, Method: {Method}",
                    requestPath,
                    requestMethod
                );

                // Restore original body stream
                context.Response.Body = originalBodyStream;

                // Re-throw to let Exception Filters or other middleware handle it
                throw;
            }
        }
    }

    /// <summary>
    /// Extension method to register middleware in Program.cs
    /// Makes it easier to use: app.UseRequestLogging() instead of app.UseMiddleware<RequestLoggingMiddleware>()
    /// </summary>
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}

