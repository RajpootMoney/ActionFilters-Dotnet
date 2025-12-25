using Microsoft.EntityFrameworkCore;
using ActionFiltersAPI.Data;
using ActionFiltersAPI.Filters;
using ActionFiltersAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Action Filters API",
        Version = "v1",
        Description = "Demonstrates Action Filters, Exception Filters, Custom Middleware, Request Pipeline, Model Binding, and Data Annotations"
    });
});

// Configure SQLite Database
// Connection string is in appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Action Filters and Exception Filters as services
// This allows them to use dependency injection
builder.Services.AddScoped<LoggingActionFilter>();
builder.Services.AddScoped<ValidationActionFilter>();
builder.Services.AddScoped<GlobalExceptionFilter>();

// Register Exception Filter globally
// This means it will catch exceptions from ALL controllers
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

var app = builder.Build();

// ============================================================================
// REQUEST PIPELINE CONFIGURATION
// ============================================================================
// The order of middleware registration is CRITICAL!
// Middleware executes in the order it's registered (request phase)
// and in reverse order (response phase)
// ============================================================================

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 1. EXCEPTION HANDLING MIDDLEWARE (should be early in pipeline)
// This catches exceptions that aren't handled by Exception Filters
app.UseExceptionHandler("/Error");

// 2. CUSTOM MIDDLEWARE - Request Logging
// This middleware logs all incoming requests and responses
// It wraps the entire request/response cycle
app.UseRequestLogging();

// 3. CUSTOM MIDDLEWARE - Performance Monitoring
// This middleware monitors request performance and adds custom headers
app.UsePerformanceMonitoring();

// 4. HTTPS Redirection (if needed)
app.UseHttpsRedirection();

// 5. Static Files (if serving static content)
// app.UseStaticFiles();

// 6. ROUTING MIDDLEWARE
// This maps incoming requests to controllers and actions
// Must be before UseAuthorization and UseEndpoints
app.UseRouting();

// 7. AUTHORIZATION MIDDLEWARE (if using authentication)
// app.UseAuthentication();
// app.UseAuthorization();

// 8. ENDPOINT MIDDLEWARE
// This executes the matched controller action
// After this, Action Filters run, then the action method
app.MapControllers();

// ============================================================================
// DATABASE SEEDING
// ============================================================================
// Seed the database with sample data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DatabaseSeeder.Seed(context);
        Console.WriteLine("Database seeded successfully!");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// ============================================================================
// REQUEST PIPELINE FLOW SUMMARY
// ============================================================================
// When a request arrives:
// 1. RequestLoggingMiddleware (logs request start)
// 2. PerformanceMiddleware (starts timer)
// 3. Routing Middleware (maps URL to controller/action)
// 4. Authorization Middleware (checks permissions)
// 5. Action Filters - OnActionExecuting (before action)
// 6. Action Method Execution
//    - Model Binding occurs here (maps request data to parameters)
//    - Data Annotations validation runs
//    - ModelState populated with validation results
// 7. Action Filters - OnActionExecuted (after action)
// 8. Exception Filters (if exception occurred)
// 9. PerformanceMiddleware (logs execution time, adds headers)
// 10. RequestLoggingMiddleware (logs response)
// 11. Response sent to client
// ============================================================================

app.Run();

