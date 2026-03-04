# Repository Pattern - Product & Order Implementation

## ✅ COMPLETED: Products and Orders Repository Pattern

---

## 📋 Summary of Changes

### 1. **Products Model** - Cleaned ✅
**Location:** `Models\Products.cs`

**BEFORE** ❌
```csharp
public class Products
{
    public int ID { get; set; }
    // ... properties
    
    public List<Products> ListData() { /* SQL */ }
    public List<Products> ListActiveMenu() { /* SQL */ }
    public bool AddProduct(...) { /* SQL */ }
    public bool UpdateProduct(...) { /* SQL */ }
    public bool DeleteProduct(...) { /* SQL */ }
    public Products SearchProductById(...) { /* SQL */ }
    public List<Products> SearchProductByName(...) { /* SQL */ }
}
```

**AFTER** ✅
```csharp
public class Products
{
    // Properties
    public int ID { get; set; }
    public int CategoryID { get; set; }
    public int SupplierID { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public string CategoryName { get; set; }
    public string SupplierName { get; set; }
    public string Status { get; set; }

    // Business Logic Methods (No SQL!)
    public decimal GetPriceAfterDiscount() { ... }
    public decimal GetDiscountPercentage() { ... }
    public bool IsActive() { ... }
    public bool HasDiscount() { ... }
    public void Activate() { ... }
    public void Deactivate() { ... }
    public void Validate() { ... }
}
```

---

### 2. **IProductRepository Interface** - Enhanced ✅
**Location:** `Interface\IProductRepository.cs`

```csharp
public interface IProductRepository
{
    List<Products> GetAllProducts();
    List<Products> GetActiveProducts();               // ✨ New
    Products GetProductById(int id);
    List<Products> SearchProducts(string keyword);
    List<Products> GetProductsByCategory(int categoryId);  // ✨ New
    List<Products> GetProductsBySupplier(int supplierId);  // ✨ New
    List<Products> GetProductsByStatus(string status);     // ✨ New
    bool AddProduct(Products product);
    bool UpdateProduct(Products product);
    bool DeleteProduct(int id);
    bool ProductExists(string name, int excludeId = 0);    // ✨ New
}
```

---

### 3. **ProductRepository** - Enhanced ✅
**Location:** `Repository\ProductsRepository.cs`

**Added Methods:**
- `GetActiveProducts()` - Get only active products
- `GetProductsByCategory(int categoryId)` - Filter by category
- `GetProductsBySupplier(int supplierId)` - Filter by supplier
- `GetProductsByStatus(string status)` - Filter by status
- `ProductExists(string name, int excludeId)` - Check duplicate names

---

### 4. **ProductService** - Enhanced with Business Logic ✅
**Location:** `Services\ProductService.cs`

**New Features:**
```csharp
public class ProductService
{
    // CRUD Operations with Validation
    public bool AddProduct(string name, int categoryId, int supplierId, 
                          decimal price, decimal discount = 0)
    {
        // ✅ Validation
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.");
        
        if (discount > price)
            throw new ArgumentException("Discount cannot exceed price.");
        
        // ✅ Check duplicates
        if (_repo.ProductExists(name))
            throw new Exception($"Product '{name}' already exists.");
        
        // ✅ Create and validate
        var product = new Products { ... };
        product.Validate();
        
        return _repo.AddProduct(product);
    }
    
    // Business Analytics Methods
    public List<Products> GetProductsWithDiscount() { ... }
    public decimal GetAveragePrice() { ... }
    public Products GetMostExpensiveProduct() { ... }
    public Products GetCheapestProduct() { ... }
    public bool ActivateProduct(int id) { ... }
    public bool DeactivateProduct(int id) { ... }
}
```

---

### 5. **Order Model** - Already Clean ✅
**Location:** `Models\Order.cs`

```csharp
public class Order
{
    public int OrderID { get; set; }
    public string Note { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public int UserID { set; get; }
    public DateTime OrderDate { get; set; }
}
```

---

### 6. **OrderService** - Created ✅
**Location:** `Services\OrderService.cs`

**Features:**
```csharp
public class OrderService
{
    // CRUD Operations
    public List<Order> GetAllOrders();
    public Order GetOrderById(int id);
    public List<Order> GetOrdersByUserId(int userId);
    public List<Order> GetOrdersByStatus(string status);
    public List<Order> GetOrdersByDateRange(DateTime start, DateTime end);
    public bool AddOrder(...);
    public bool UpdateOrder(...);
    public bool UpdateOrderStatus(int orderId, string newStatus);
    
    // Status Management
    public bool CompleteOrder(int orderId);
    public bool CancelOrder(int orderId);
    public bool ProcessOrder(int orderId);
    
    // Business Analytics
    public List<Order> GetPendingOrders();
    public List<Order> GetCompletedOrders();
    public List<Order> GetTodayOrders();
    public List<Order> GetThisWeekOrders();
    public List<Order> GetThisMonthOrders();
    
    // Revenue Analytics
    public decimal GetTotalRevenue();
    public decimal GetTotalRevenueByDateRange(DateTime start, DateTime end);
    public decimal GetAverageOrderValue();
    public int GetCompletedOrderCount();
    public Order GetLargestOrder();
}
```

---

### 7. **UI Updates** ✅

#### AdminAddProducts.cs
**BEFORE** ❌
```csharp
public partial class AdminAddProducts : UserControl
{
    private void btnAdd_Click(...)
    {
        Products products = new Products();
        products.AddProduct(...);  // ❌ Direct model usage
    }
}
```

**AFTER** ✅
```csharp
public partial class AdminAddProducts : UserControl
{
    private readonly ProductService _productService;
    
    public AdminAddProducts()
    {
        var repo = new ProductRepository();
        _productService = new ProductService(repo);  // ✅ Dependency Injection
    }
    
    private void btnAdd_Click(...)
    {
        _productService.AddProduct(...);  // ✅ Service layer
    }
}
```

#### POSForm.cs
```csharp
public partial class POSForm : UserControl
{
    private OrderManager _orderManager;
    private ProductService _productService;  // ✅ Added
    
    public POSForm()
    {
        _orderManager = new OrderManager();
        _productService = new ProductService(new ProductRepository());  // ✅ Added
    }
    
    private void ListMenu()
    {
        List<Products> productsList = _productService.GetActiveProducts();  // ✅
        dgvListMenu.DataSource = productsList;
    }
}
```

---

## 🏗️ Complete Architecture

```
┌─────────────────────────────────────────────┐
│           UI Layer (Views)                  │
│  - AdminAddProducts.cs                      │
│  - POSForm.cs                               │
│  - AdminAddUser.cs                          │
└──────────────┬──────────────────────────────┘
               │ uses
               ▼
┌─────────────────────────────────────────────┐
│      Service Layer (Business Logic)         │
│  - ProductService.cs  ✅                     │
│  - OrderService.cs    ✅ NEW                 │
│  - UserService.cs     ✅                     │
│                                              │
│  Responsibilities:                           │
│  • Validation                                │
│  • Business Rules                            │
│  • Orchestration                             │
│  • Analytics                                 │
└──────────────┬──────────────────────────────┘
               │ uses
               ▼
┌─────────────────────────────────────────────┐
│    Repository Layer (Data Access)           │
│  - ProductRepository  ✅ Enhanced            │
│  - OrderRepository    ✅                     │
│  - UserRepository     ✅                     │
│                                              │
│  Responsibilities:                           │
│  • SQL Queries                               │
│  • Data Mapping                              │
│  • CRUD Operations                           │
└──────────────┬──────────────────────────────┘
               │ uses
               ▼
┌─────────────────────────────────────────────┐
│        Database Layer                        │
│  - DbConnection.cs (Singleton Pattern)       │
└─────────────────────────────────────────────┘
```

---

## 📊 Pattern Comparison

| Aspect | Before | After |
|--------|--------|-------|
| **Models** | Contains SQL | Pure POCOs with business logic |
| **Data Access** | Scattered in models | Centralized in Repositories |
| **Validation** | In UI or missing | Service layer |
| **Testability** | Difficult | Easy with mocks |
| **Maintainability** | Low | High |
| **Code Reuse** | Duplicated code | Reusable services |

---

## 🎯 Benefits Achieved

### 1. **Clean Separation**
- ✅ Models: Domain logic only
- ✅ Repositories: Data access only
- ✅ Services: Business logic and validation
- ✅ UI: Presentation only

### 2. **Enhanced Features**

**ProductService Analytics:**
```csharp
var service = new ProductService(new ProductRepository());

// Get insights
var avgPrice = service.GetAveragePrice();
var mostExpensive = service.GetMostExpensiveProduct();
var withDiscounts = service.GetProductsWithDiscount();
```

**OrderService Analytics:**
```csharp
var orderService = new OrderService(new OrderRepository());

// Business insights
var todayRevenue = orderService.GetTotalRevenueByDateRange(
    DateTime.Today, DateTime.Now);
var avgOrderValue = orderService.GetAverageOrderValue();
var todayOrders = orderService.GetTodayOrders();
```

### 3. **Better Error Handling**
```csharp
// Service validates before repository
try
{
    service.AddProduct("Coffee", 1, 1, 5.99m, 0.50m);
}
catch (ArgumentException ex)
{
    // Validation error: "Discount cannot be negative"
}
catch (Exception ex)
{
    // Business rule error: "Product 'Coffee' already exists"
}
```

---

## 📝 Usage Examples

### Product Operations

```csharp
// Initialize service
var productService = new ProductService(new ProductRepository());

// Add product
productService.AddProduct(
    name: "Espresso",
    categoryId: 1,
    supplierId: 2,
    price: 3.50m,
    discount: 0.25m
);

// Search products
var results = productService.SearchProducts("coffee");

// Get by category
var beverages = productService.GetProductsByCategory(1);

// Analytics
var avgPrice = productService.GetAveragePrice();
var mostExpensive = productService.GetMostExpensiveProduct();

// Activate/Deactivate
productService.ActivateProduct(5);
productService.DeactivateProduct(10);
```

### Order Operations

```csharp
// Initialize service
var orderService = new OrderService(new OrderRepository());

// Create order
orderService.AddOrder(
    userId: UserSession.UserId,
    totalAmount: 25.50m,
    discount: 2.00m,
    note: "Extra hot",
    status: "Pending"
);

// Update status
orderService.CompleteOrder(orderId: 123);
orderService.CancelOrder(orderId: 124);

// Get orders
var myOrders = orderService.GetOrdersByUserId(UserSession.UserId);
var todayOrders = orderService.GetTodayOrders();
var pending = orderService.GetPendingOrders();

// Analytics
var totalRevenue = orderService.GetTotalRevenue();
var thisMonthRevenue = orderService.GetTotalRevenueByDateRange(
    new DateTime(2024, 1, 1),
    new DateTime(2024, 1, 31)
);
var avgOrder = orderService.GetAverageOrderValue();
```

---

## ✅ Checklist

- [x] Products Model - Cleaned (no SQL)
- [x] Order Model - Cleaned (no SQL)
- [x] IProductRepository - Enhanced with new methods
- [x] IOrderRepository - Created
- [x] ProductRepository - Enhanced
- [x] OrderRepository - Created
- [x] ProductService - Enhanced with validation & analytics
- [x] OrderService - Created with full features
- [x] AdminAddProducts.cs - Updated to use ProductService
- [x] POSForm.cs - Updated to use ProductService
- [x] Build successful ✅

---

## 🚀 Next Pattern: Factory Pattern

**Ready to implement Pattern #2?**

We can create:
1. **UserFactory** - Create different user types (Admin, Cashier, Manager)
2. **OrderFactory** - Create different order types (Dine-in, Takeaway, Delivery)
3. **ProductFactory** - Create different product categories

Let me know when you're ready!

---

**Repository Pattern: Products & Orders** ✅ **COMPLETED**
