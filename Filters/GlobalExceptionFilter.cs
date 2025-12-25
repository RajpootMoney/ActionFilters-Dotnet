using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace ActionFiltersAPI.Filters
{
    /// <summary>
    /// EXCEPTION FILTER EXAMPLE
    /// 
    /// Exception Filters handle exceptions that occur during:
    /// - Action execution
    /// - Action filters
    /// - Model binding
    /// - Authorization filters
    /// 
    /// Exception Filters run AFTER the exception occurs but BEFORE the response is sent.
    /// They can catch, log, and transform exceptions into appropriate HTTP responses.
    /// 
    /// Execution Order:
    /// 1. Action executes (exception thrown)
    /// 2. Exception Filters run (this filter)
    /// 3. If exception not handled, it bubbles up to middleware
    /// 
    /// This filter provides centralized exception handling for the entire API.
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionFilter(
            ILogger<GlobalExceptionFilter> logger,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        /// OnException: Called when an unhandled exception occurs
        /// This is where you can:
        /// - Log the exception
        /// - Transform exception into appropriate HTTP response
        /// - Prevent sensitive error details from leaking to clients
        /// </summary>
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var requestPath = context.HttpContext.Request.Path;
            var requestMethod = context.HttpContext.Request.Method;

            // Log the exception with full details
            _logger.LogError(
                exception,
                "EXCEPTION FILTER - Unhandled exception occurred. " +
                "Path: {Path}, Method: {Method}, Exception: {ExceptionType}",
                requestPath,
                requestMethod,
                exception.GetType().Name
            );

            // Create appropriate response based on exception type
            var response = exception switch
            {
                ArgumentNullException => new ObjectResult(new
                {
                    error = "Invalid request",
                    message = "Required parameter is missing",
                    statusCode = (int)HttpStatusCode.BadRequest
                })
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                },

                ArgumentException => new ObjectResult(new
                {
                    error = "Invalid argument",
                    message = exception.Message,
                    statusCode = (int)HttpStatusCode.BadRequest
                })
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                },

                KeyNotFoundException => new ObjectResult(new
                {
                    error = "Resource not found",
                    message = "The requested resource was not found",
                    statusCode = (int)HttpStatusCode.NotFound
                })
                {
                    StatusCode = (int)HttpStatusCode.NotFound
                },

                UnauthorizedAccessException => new ObjectResult(new
                {
                    error = "Unauthorized",
                    message = "You do not have permission to access this resource",
                    statusCode = (int)HttpStatusCode.Unauthorized
                })
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized
                },

                // Default: Generic server error
                _ => new ObjectResult(new
                {
                    error = "Internal server error",
                    message = _environment.IsDevelopment() 
                        ? exception.Message 
                        : "An error occurred while processing your request",
                    // Only include stack trace in development
                    stackTrace = _environment.IsDevelopment() ? exception.StackTrace : null,
                    statusCode = (int)HttpStatusCode.InternalServerError
                })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                }
            };

            // Set the result - this prevents the exception from propagating
            context.Result = response;
            
            // Mark exception as handled - prevents it from bubbling up
            context.ExceptionHandled = true;

            _logger.LogInformation(
                "EXCEPTION FILTER - Exception handled. " +
                "Response Status: {StatusCode}",
                response.StatusCode
            );
        }
    }
}

