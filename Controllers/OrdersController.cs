using ActionFiltersAPI.Data;
using ActionFiltersAPI.Filters;
using ActionFiltersAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ActionFiltersAPI.Controllers
{
    /// <summary>
    /// Orders Controller
    ///
    /// Demonstrates:
    /// - Complex model binding (nested objects)
    /// - Multiple validation scenarios
    /// - Exception handling (will be caught by Exception Filter)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    //[ServiceFilter(typeof(LoggingActionFilter))]
    //[ServiceFilter(typeof(ValidationActionFilter))]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ApplicationDbContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// GET /api/orders
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _context
                .Orders.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            return Ok(orders);
        }

        /// <summary>
        /// GET /api/orders/5
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context
                .Orders.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { message = $"Order with ID {id} not found" });
            }

            return Ok(order);
        }

        /// <summary>
        /// POST /api/orders
        ///
        /// COMPLEX MODEL BINDING EXAMPLE:
        /// - Order object with nested OrderItems collection
        /// - Framework automatically binds nested JSON structure
        ///
        /// VALIDATION EXAMPLE:
        /// - Order properties validated (CustomerName, Email, etc.)
        /// - OrderItem properties validated (Quantity, UnitPrice, etc.)
        /// - All validation errors collected in ModelState
        ///
        /// EXCEPTION HANDLING EXAMPLE:
        /// - If product not found, KeyNotFoundException thrown
        /// - Exception Filter will catch and return appropriate response
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            // MODEL BINDING:
            // Complex nested JSON like this is automatically bound:
            // {
            //   "customerName": "John Doe",
            //   "customerEmail": "john@example.com",
            //   "orderItems": [
            //     { "productId": 1, "quantity": 2, "unitPrice": 10.50 }
            //   ]
            // }

            // VALIDATION:
            // All Data Annotations on Order and OrderItem are validated
            // ValidationActionFilter checks ModelState before this executes

            // Calculate total amount
            decimal totalAmount = 0;

            foreach (var item in order.OrderItems)
            {
                // Verify product exists
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    // This exception will be caught by GlobalExceptionFilter
                    throw new KeyNotFoundException($"Product with ID {item.ProductId} not found");
                }

                // Use product price if unit price not provided
                if (item.UnitPrice == 0)
                {
                    item.UnitPrice = product.Price;
                }

                totalAmount += item.UnitPrice * item.Quantity;
            }

            order.TotalAmount = totalAmount;
            order.OrderDate = DateTime.Now;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Load related data for response
            await _context
                .Entry(order)
                .Collection(o => o.OrderItems)
                .Query()
                .Include(oi => oi.Product)
                .LoadAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        /// <summary>
        /// PUT /api/orders/5/status
        ///
        /// MODEL BINDING: Route parameter + query parameter
        /// Example: PUT /api/orders/5/status?status=Shipped
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromQuery] OrderStatus status)
        {
            // MODEL BINDING:
            // - "id" from route: /api/orders/5/status
            // - "status" from query string: ?status=Shipped
            // Enum binding: String "Shipped" automatically converted to OrderStatus enum

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// DELETE /api/orders/5
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context
                .Orders.Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
