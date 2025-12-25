using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionFiltersAPI.Filters
{
    /// <summary>
    /// ACTION FILTER EXAMPLE - Model Validation
    /// 
    /// This filter demonstrates how to check ModelState before action execution.
    /// ModelState contains validation errors from Data Annotations.
    /// 
    /// Model Binding Process:
    /// 1. Framework receives HTTP request
    /// 2. Model Binding maps request data (query string, route, body) to action parameters
    /// 3. Data Annotations are validated during binding
    /// 4. Results stored in ModelState
    /// 5. Action Filter can check ModelState before action runs
    /// </summary>
    public class ValidationActionFilter : IActionFilter
    {
        private readonly ILogger<ValidationActionFilter> _logger;

        public ValidationActionFilter(ILogger<ValidationActionFilter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Check ModelState before action executes
        /// If ModelState is invalid, we can short-circuit and return BadRequest
        /// </summary>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // ModelState contains validation results from Data Annotations
            // It's populated during Model Binding phase
            if (!context.ModelState.IsValid)
            {
                _logger.LogWarning(
                    "VALIDATION FILTER - Model validation failed. " +
                    "Errors: {Errors}",
                    string.Join(", ", context.ModelState
                        .SelectMany(x => x.Value?.Errors ?? Enumerable.Empty<Microsoft.AspNetCore.Mvc.ModelBinding.ModelError>())
                        .Select(e => e.ErrorMessage))
                );

                // Short-circuit the request - action method won't execute
                // Return BadRequest with validation errors
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
            else
            {
                _logger.LogInformation(
                    "VALIDATION FILTER - Model validation passed. " +
                    "All Data Annotation validations succeeded."
                );
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // This runs after action execution
            // Could log successful validations here if needed
        }
    }
}

