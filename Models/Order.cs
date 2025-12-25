using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActionFiltersAPI.Models
{
    /// <summary>
    /// Order model demonstrating complex validation scenarios
    /// </summary>
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters")]
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// EmailAddress attribute validates email format
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string CustomerEmail { get; set; } = string.Empty;

        /// <summary>
        /// Phone attribute validates phone number format
        /// </summary>
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? CustomerPhone { get; set; }

        [Required(ErrorMessage = "Order date is required")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Range validation for decimal total amount
        /// </summary>
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Enum validation - OrderStatus enum
        /// </summary>
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Navigation property
        public List<OrderItem> OrderItems { get; set; } = new();
    }

    /// <summary>
    /// Enum for order status - demonstrates enum validation
    /// </summary>
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
}

