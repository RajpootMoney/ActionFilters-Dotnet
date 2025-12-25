# Action Filters API - .NET Core Web API

This project demonstrates key ASP.NET Core concepts including Action Filters, Exception Filters, Custom Middleware, Request Pipeline, Model Binding, and Data Annotations validation.

## Features Demonstrated

### 1. **Action Filters**
- `LoggingActionFilter`: Logs action execution time and details
- `ValidationActionFilter`: Validates ModelState before action execution
- Applied at controller level using `[ServiceFilter]` attribute

### 2. **Exception Filters**
- `GlobalExceptionFilter`: Centralized exception handling
- Catches and transforms exceptions into appropriate HTTP responses
- Registered globally for all controllers

### 3. **Custom Middleware**
- `RequestLoggingMiddleware`: Logs all HTTP requests and responses
- `PerformanceMiddleware`: Monitors request performance and adds custom headers
- Demonstrates request/response pipeline flow

### 4. **Request Pipeline**
- Configured in `Program.cs` with detailed comments
- Shows middleware execution order
- Demonstrates how components interact

### 5. **Model Binding**
- Automatic binding from route parameters, query strings, and request body
- Complex nested object binding
- Enum binding examples

### 6. **Data Annotations Validation**
- `Product` model with comprehensive validation attributes
- `Order` model with email, phone, and enum validation
- Automatic validation during model binding
- Validation errors returned via `ModelState`

## Database

- **SQLite** database (`ActionFiltersDB.db`)
- Entity Framework Core for data access
- Sample data seeded on startup

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later

### Running the Application

1. Restore packages:
```bash
dotnet restore
```

2. Run the application:
```bash
dotnet run
```

3. Open Swagger UI:
```
https://localhost:5001/swagger
```
or
```
http://localhost:5000/swagger
```

## API Endpoints

### Products
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product (with validation)
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

### Orders
- `GET /api/orders` - Get all orders
- `GET /api/orders/{id}` - Get order by ID
- `POST /api/orders` - Create new order (with validation)
- `PUT /api/orders/{id}/status` - Update order status
- `DELETE /api/orders/{id}` - Delete order

### Test Endpoints
- `GET /api/test/model-binding-demo` - Test model binding
- `POST /api/test/validation-demo` - Test validation
- `GET /api/test/exception-demo` - Test exception handling
- `GET /api/test/pipeline-info` - Get pipeline information

## Testing Concepts

### Test Model Binding
```bash
GET /api/test/model-binding-demo?name=John&age=30&city=NewYork
```

### Test Validation
```bash
POST /api/test/validation-demo
Content-Type: application/json

{
  "name": "AB",  // Too short - will fail validation
  "price": -10,  // Negative - will fail validation
  "sku": "INVALID"  // Wrong format - will fail validation
}
```

### Test Exception Filter
```bash
GET /api/test/exception-demo?exceptionType=notfound
GET /api/test/exception-demo?exceptionType=argumentnull
GET /api/test/exception-demo?exceptionType=generic
```

### Test Product Creation (with validation)
```bash
POST /api/products
Content-Type: application/json

{
  "name": "New Product",
  "price": 99.99,
  "sku": "NP-1234",
  "description": "A new product",
  "stockQuantity": 100
}
```

## Code Structure

```
ActionFiltersAPI/
├── Controllers/          # API controllers
├── Data/                # DbContext and database seeding
├── Filters/             # Action and Exception filters
├── Middleware/          # Custom middleware components
├── Models/              # Entity models with Data Annotations
├── Program.cs           # Application startup and pipeline configuration
└── appsettings.json     # Configuration
```

## Key Concepts Explained

All code includes detailed comments explaining:
- How Action Filters work and when they execute
- How Exception Filters catch and handle errors
- How Middleware fits into the request pipeline
- How Model Binding maps request data to parameters
- How Data Annotations provide validation
- The complete request pipeline flow

## Notes

- Check console logs to see middleware and filter execution
- Validation errors are returned as BadRequest with ModelState
- Exceptions are caught by Exception Filter and returned as appropriate HTTP responses
- All requests are logged by custom middleware
- Performance metrics are added as response headers (`X-Execution-Time-Ms`)

