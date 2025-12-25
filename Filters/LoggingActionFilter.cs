using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace ActionFiltersAPI.Filters
{
    /// <summary>
    /// ACTION FILTER EXAMPLE
    /// 
    /// Action Filters are attributes that can be applied to controllers or action methods.
    /// They allow you to run code before and after action methods execute.
    /// 
    /// Execution Order in Request Pipeline:
    /// 1. Authorization Filters (run first)
    /// 2. Resource Filters
    /// 3. Action Filters (this is where we are)
    /// 4. Exception Filters
    /// 5. Result Filters
    /// 
    /// This filter logs the execution time of action methods.
    /// </summary>
    public class LoggingActionFilter : IActionFilter
    {
        private readonly ILogger<LoggingActionFilter> _logger;
        private Stopwatch? _stopwatch;

        public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// OnActionExecuting: Called BEFORE the action method executes
        /// This is where you can:
        /// - Validate request data
        /// - Log incoming requests
        /// - Modify action arguments
        /// - Short-circuit the request (by setting context.Result)
        /// </summary>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();
            
            var actionName = context.ActionDescriptor.DisplayName;
            var controllerName = context.RouteData.Values["controller"];
            var actionMethod = context.RouteData.Values["action"];
            
            _logger.LogInformation(
                "ACTION FILTER - OnActionExecuting: " +
                "Controller: {Controller}, Action: {Action}, " +
                "Method: {Method}, Path: {Path}",
                controllerName,
                actionMethod,
                context.HttpContext.Request.Method,
                context.HttpContext.Request.Path
            );

            // Example: You can short-circuit the request here if needed
            // if (someCondition)
            // {
            //     context.Result = new BadRequestObjectResult("Request blocked by filter");
            //     return;
            // }
        }

        /// <summary>
        /// OnActionExecuted: Called AFTER the action method executes
        /// This is where you can:
        /// - Log response data
        /// - Modify the result
        /// - Handle exceptions
        /// - Perform cleanup
        /// </summary>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch?.Stop();
            
            var controllerName = context.RouteData.Values["controller"];
            var actionMethod = context.RouteData.Values["action"];
            var executionTime = _stopwatch?.ElapsedMilliseconds ?? 0;
            
            if (context.Exception != null)
            {
                _logger.LogError(
                    "ACTION FILTER - OnActionExecuted (with exception): " +
                    "Controller: {Controller}, Action: {Action}, " +
                    "Exception: {Exception}, ExecutionTime: {Time}ms",
                    controllerName,
                    actionMethod,
                    context.Exception.Message,
                    executionTime
                );
            }
            else
            {
                _logger.LogInformation(
                    "ACTION FILTER - OnActionExecuted: " +
                    "Controller: {Controller}, Action: {Action}, " +
                    "Status: {Status}, ExecutionTime: {Time}ms",
                    controllerName,
                    actionMethod,
                    context.HttpContext.Response.StatusCode,
                    executionTime
                );
            }
        }
    }
}

