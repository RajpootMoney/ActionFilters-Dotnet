using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActionFiltersAPI.Models
{
    /// <summary>
    /// Product model demonstrating Data Annotations for validation
    /// Data Annotations are attributes that provide declarative validation rules
    /// They are checked during Model Binding and Model Validation phases
    /// </summary>
    public class Product
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Required attribute ensures the property must have a value
        /// Model binding will fail if this is null or empty
        /// </summary>
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Range attribute validates numeric values are within specified bounds
        /// </summary>
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between $0.01 and $10,000.00")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// RegularExpression validates string format matches a pattern
        /// </summary>
        [Required(ErrorMessage = "SKU is required")]
        [RegularExpression(@"^[A-Z]{2}-\d{4}$", ErrorMessage = "SKU must be in format: XX-1234 (e.g., AB-1234)")]
        public string SKU { get; set; } = string.Empty;

        /// <summary>
        /// StringLength limits the maximum length of a string
        /// </summary>
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Range attribute for integer values
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int StockQuantity { get; set; }

        /// <summary>
        /// DataType attribute helps with formatting and validation
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}

