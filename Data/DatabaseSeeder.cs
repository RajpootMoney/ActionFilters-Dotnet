using ActionFiltersAPI.Models;

namespace ActionFiltersAPI.Data
{
    /// <summary>
    /// Database Seeder
    /// Seeds sample data into the database for testing and demonstration
    /// </summary>
    public static class DatabaseSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if data already exists
            if (context.Products.Any())
            {
                return; // Database already seeded
            }

            // Seed Products
            var products = new List<Product>
            {
                new Product
                {
                    Name = "Laptop Computer",
                    Price = 999.99m,
                    SKU = "LT-1001",
                    Description = "High-performance laptop with 16GB RAM and 512GB SSD",
                    StockQuantity = 50,
                    CreatedDate = DateTime.Now.AddDays(-30),
                    IsActive = true
                },
                new Product
                {
                    Name = "Wireless Mouse",
                    Price = 29.99m,
                    SKU = "MS-2001",
                    Description = "Ergonomic wireless mouse with long battery life",
                    StockQuantity = 200,
                    CreatedDate = DateTime.Now.AddDays(-20),
                    IsActive = true
                },
                new Product
                {
                    Name = "Mechanical Keyboard",
                    Price = 149.99m,
                    SKU = "KB-3001",
                    Description = "RGB mechanical keyboard with Cherry MX switches",
                    StockQuantity = 75,
                    CreatedDate = DateTime.Now.AddDays(-15),
                    IsActive = true
                },
                new Product
                {
                    Name = "USB-C Hub",
                    Price = 49.99m,
                    SKU = "HB-4001",
                    Description = "7-in-1 USB-C hub with HDMI, USB 3.0, and SD card reader",
                    StockQuantity = 120,
                    CreatedDate = DateTime.Now.AddDays(-10),
                    IsActive = true
                },
                new Product
                {
                    Name = "Monitor Stand",
                    Price = 79.99m,
                    SKU = "ST-5001",
                    Description = "Adjustable monitor stand with cable management",
                    StockQuantity = 90,
                    CreatedDate = DateTime.Now.AddDays(-5),
                    IsActive = true
                }
            };

            context.Products.AddRange(products);
            context.SaveChanges();

            // Seed Orders with OrderItems
            var orders = new List<Order>
            {
                new Order
                {
                    CustomerName = "John Doe",
                    CustomerEmail = "john.doe@example.com",
                    CustomerPhone = "+1-555-0101",
                    OrderDate = DateTime.Now.AddDays(-5),
                    TotalAmount = 1029.98m,
                    Status = OrderStatus.Delivered,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ProductId = products[0].Id, // Laptop
                            Quantity = 1,
                            UnitPrice = 999.99m
                        },
                        new OrderItem
                        {
                            ProductId = products[1].Id, // Mouse
                            Quantity = 1,
                            UnitPrice = 29.99m
                        }
                    }
                },
                new Order
                {
                    CustomerName = "Jane Smith",
                    CustomerEmail = "jane.smith@example.com",
                    CustomerPhone = "+1-555-0102",
                    OrderDate = DateTime.Now.AddDays(-3),
                    TotalAmount = 199.98m,
                    Status = OrderStatus.Shipped,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ProductId = products[2].Id, // Keyboard
                            Quantity = 1,
                            UnitPrice = 149.99m
                        },
                        new OrderItem
                        {
                            ProductId = products[3].Id, // USB-C Hub
                            Quantity = 1,
                            UnitPrice = 49.99m
                        }
                    }
                },
                new Order
                {
                    CustomerName = "Bob Johnson",
                    CustomerEmail = "bob.johnson@example.com",
                    CustomerPhone = "+1-555-0103",
                    OrderDate = DateTime.Now.AddDays(-1),
                    TotalAmount = 79.99m,
                    Status = OrderStatus.Processing,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ProductId = products[4].Id, // Monitor Stand
                            Quantity = 1,
                            UnitPrice = 79.99m
                        }
                    }
                }
            };

            context.Orders.AddRange(orders);
            context.SaveChanges();
        }
    }
}

