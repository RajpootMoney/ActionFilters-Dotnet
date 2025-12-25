using Microsoft.AspNetCore.Mvc;
using ActionFiltersAPI.Filters;

namespace ActionFiltersAPI.Controllers
{
    /// <summary>
    /// Test Controller
    /// 
    /// This controller demonstrates various scenarios to test:
    /// - Model Binding
    /// - Validation
    /// - Action Filters
    /// - Exception Filters
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// GET /api/test/model-binding-demo?name=John&age=30
        /// 
        /// MODEL BINDING DEMO:
        /// - Query parameters automatically bound to action parameters
        /// - No [FromQuery] needed for simple types (convention-based)
        /// </summary>
        [HttpGet("model-binding-demo")]
        [ServiceFilter(typeof(LoggingActionFilter))]
        public IActionResult ModelBindingDemo(
            string name,        // Bound from ?name=John
            int age,            // Bound from ?age=30
            [FromQuery] string? city = null) // Explicit [FromQuery] with default
        {
            return Ok(new
            {
                message = "Model Binding Demo",
                boundData = new
                {
                    name,
                    age,
                    city
                },
                explanation = "Parameters automatically bound from query string"
            });
        }

        /// <summary>
        /// POST /api/test/validation-demo
        /// 
        /// VALIDATION DEMO:
        /// - Send invalid data to see validation errors
        /// - ValidationActionFilter will catch and return BadRequest
        /// </summary>
        [HttpPost("validation-demo")]
        [ServiceFilter(typeof(ValidationActionFilter))]
        public IActionResult ValidationDemo([FromBody] ActionFiltersAPI.Models.Product product)
        {
            // If we reach here, validation passed
            return Ok(new
            {
                message = "Validation passed!",
                product
            });
        }

        /// <summary>
        /// GET /api/test/exception-demo
        /// 
        /// EXCEPTION FILTER DEMO:
        /// - Throws different exceptions to test Exception Filter
        /// </summary>
        [HttpGet("exception-demo")]
        public IActionResult ExceptionDemo([FromQuery] string? exceptionType = "generic")
        {
            // Exception Filter will catch these and return appropriate responses
            return exceptionType.ToLower() switch
            {
                "argumentnull" => throw new ArgumentNullException(nameof(exceptionType), "Test ArgumentNullException"),
                "argument" => throw new ArgumentException("Test ArgumentException", nameof(exceptionType)),
                "notfound" => throw new KeyNotFoundException("Test KeyNotFoundException - Resource not found"),
                "unauthorized" => throw new UnauthorizedAccessException("Test UnauthorizedAccessException"),
                _ => throw new Exception("Test generic exception - This will return 500 Internal Server Error")
            };
        }

        /// <summary>
        /// GET /api/test/pipeline-info
        /// 
        /// Returns information about the request pipeline
        /// </summary>
        [HttpGet("pipeline-info")]
        public IActionResult PipelineInfo()
        {
            return Ok(new
            {
                message = "Request Pipeline Information",
                pipeline = new[]
                {
                    "1. HTTP Request arrives",
                    "2. Custom Middleware (RequestLoggingMiddleware) - Logs request",
                    "3. Custom Middleware (PerformanceMiddleware) - Monitors performance",
                    "4. Routing Middleware - Maps URL to controller/action",
                    "5. Authorization Middleware (if configured)",
                    "6. Action Filters (OnActionExecuting) - Before action",
                    "7. Action Method Execution",
                    "8. Action Filters (OnActionExecuted) - After action",
                    "9. Exception Filters (if exception occurs)",
                    "10. Response Middleware (reverse order)",
                    "11. HTTP Response sent"
                },
                currentRequest = new
                {
                    method = HttpContext.Request.Method,
                    path = HttpContext.Request.Path,
                    query = HttpContext.Request.QueryString.ToString(),
                    headers = HttpContext.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
                }
            });
        }
    }
}

