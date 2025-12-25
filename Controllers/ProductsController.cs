using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ActionFiltersAPI.Data;
using ActionFiltersAPI.Models;
using ActionFiltersAPI.Filters;

namespace ActionFiltersAPI.Controllers
{
    /// <summary>
    /// Products Controller
    /// 
    /// This controller demonstrates:
    /// 1. Model Binding - How request data maps to action parameters
    /// 2. Data Annotations Validation - Automatic validation using attributes
    /// 3. Action Filters - Applied at controller level (affects all actions)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(LoggingActionFilter))] // Apply Action Filter to all actions in this controller
    [ServiceFilter(typeof(ValidationActionFilter))] // Apply Validation Filter to all actions
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            ApplicationDbContext context,
            ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// GET /api/products
        /// 
        /// MODEL BINDING: No model binding here, just querying database
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            // Action Filter will log before and after this executes
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        /// <summary>
        /// GET /api/products/5
        /// 
        /// MODEL BINDING: Route parameter "id" is automatically bound from URL
        /// Example: /api/products/5 -> id = 5
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            // Model Binding: The "id" parameter is automatically extracted from route
            // No manual parsing needed - ASP.NET Core handles it
            
            var product = await _context.Products.FindAsync(id);
            
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found" });
            }

            return Ok(product);
        }

        /// <summary>
        /// POST /api/products
        /// 
        /// MODEL BINDING EXAMPLE:
        /// - Request body (JSON) is automatically deserialized to Product object
        /// - Framework maps JSON properties to Product properties
        /// 
        /// VALIDATION EXAMPLE:
        /// - Data Annotations on Product model are automatically validated
        /// - ValidationActionFilter checks ModelState before action executes
        /// - If validation fails, BadRequest is returned (by filter)
        /// - If validation passes, action executes normally
        /// 
        /// REQUEST PIPELINE FLOW:
        /// 1. Request arrives
        /// 2. Middleware processes request
        /// 3. Routing middleware maps to this action
        /// 4. Model Binding: JSON -> Product object
        /// 5. Validation: Data Annotations checked, results in ModelState
        /// 6. Action Filter (ValidationActionFilter): Checks ModelState
        /// 7. Action Filter (LoggingActionFilter): Logs execution
        /// 8. Action method executes (if ModelState is valid)
        /// 9. Response returned
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            // MODEL BINDING:
            // [FromBody] tells framework to bind from request body
            // Framework automatically:
            // - Deserializes JSON to Product object
            // - Validates using Data Annotations
            // - Populates ModelState with validation results
            
            // VALIDATION:
            // At this point, if we reach here, ModelState is valid
            // (ValidationActionFilter would have short-circuited if invalid)
            // But we can also manually check:
            if (!ModelState.IsValid)
            {
                // This shouldn't happen if filter works correctly, but good practice
                return BadRequest(ModelState);
            }

            // Add to database
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Return created product with 201 status
            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                product
            );
        }

        /// <summary>
        /// PUT /api/products/5
        /// 
        /// MODEL BINDING:
        /// - Route parameter "id" from URL
        /// - Request body "product" from JSON
        /// 
        /// VALIDATION:
        /// - Product model validation runs automatically
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            // Model Binding: Both "id" (from route) and "product" (from body) are bound
            
            if (id != product.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            // Validation happens automatically via Data Annotations
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                throw; // Exception Filter will catch this
            }

            return NoContent();
        }

        /// <summary>
        /// DELETE /api/products/5
        /// 
        /// MODEL BINDING: Route parameter "id"
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}

