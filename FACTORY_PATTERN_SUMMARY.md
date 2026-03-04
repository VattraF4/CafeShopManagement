# Factory Pattern Implementation - Complete Guide

## ✅ PATTERN #2: FACTORY PATTERN - COMPLETED

---

## 🎯 Overview

The Factory Pattern has been successfully implemented across your entire Cafe Shop Management project. This pattern centralizes object creation and provides a clean, maintainable way to create complex objects.

---

## 📁 Factory Classes Created

### 1. **UserFactory** ✅
**Location:** `Factory\UserFactory.cs`

**Purpose:** Creates different types of users with proper initialization

**Features:**
- ✅ Create users by role (Admin, Cashier, Manager, Staff)
- ✅ Specialized methods for each user type
- ✅ Validation built-in
- ✅ Consistent initialization

**Methods:**
```csharp
// Generic method
Users CreateUser(string username, string password, string role)

// Specialized methods
Users CreateAdmin(string username, string password)
Users CreateCashier(string username, string password)
Users CreateManager(string username, string password)
Users CreateStaff(string username, string password)

// For database loading
Users CreateFromData(int id, string username, string hashedPassword, ...)
```

---

### 2. **OrderFactory** ✅
**Location:** `Factory\OrderFactory.cs`

**Purpose:** Creates different types of orders (Dine-in, Takeaway, Delivery)

**Features:**
- ✅ Create standard, pending, and completed orders
- ✅ Specialized order types (Dine-in, Takeaway, Delivery)
- ✅ Automatic status and date initialization
- ✅ Built-in validation

**Methods:**
```csharp
// Standard methods
Order CreateOrder(int userId, string orderType = "Standard")
Order CreatePendingOrder(int userId, decimal totalAmount, decimal discount = 0, string note = null)
Order CreateCompletedOrder(int userId, decimal totalAmount, decimal discount = 0, string note = null)

// Specialized order types
Order CreateDineInOrder(int userId, int tableNumber)
Order CreateTakeawayOrder(int userId, string customerName = null)
Order CreateDeliveryOrder(int userId, string deliveryAddress, string customerName = null, string phone = null)

// For database loading
Order CreateFromData(int orderId, int userId, string status, ...)
```

---

### 3. **ProductFactory** ✅
**Location:** `Factory\ProductFactory.cs`

**Purpose:** Creates products with proper validation and initialization

**Features:**
- ✅ Create products with full validation
- ✅ Category-specific product creation (Beverage, Food, Dessert)
- ✅ Percentage discount calculation
- ✅ Promotional products support
- ✅ Active/Inactive product creation

**Methods:**
```csharp
// Standard methods
Products CreateProduct(string name, int categoryId, int supplierId, decimal price, decimal discount = 0)
Products CreateActiveProduct(...)
Products CreateInactiveProduct(...)

// Category-specific methods
Products CreateBeverageProduct(string name, int supplierId, decimal price, decimal discount = 0)
Products CreateFoodProduct(string name, int supplierId, decimal price, decimal discount = 0)
Products CreateDessertProduct(string name, int supplierId, decimal price, decimal discount = 0)

// Advanced methods
Products CreateProductWithPercentDiscount(string name, int categoryId, int supplierId, decimal price, decimal discountPercent)
Products CreatePromotionalProduct(...)

// For database loading
Products CreateFromData(int id, string name, ...)
```

---

### 4. **RepositoryFactory** ✅ (Singleton)
**Location:** `Factory\RepositoryFactory.cs`

**Purpose:** Centralized repository creation with caching

**Features:**
- ✅ Singleton pattern for factory itself
- ✅ Cached repository instances (performance optimization)
- ✅ Thread-safe implementation
- ✅ Option to create new instances

**Methods:**
```csharp
// Singleton access
RepositoryFactory.Instance

// Cached repository creation (recommended)
IUserRepository CreateUserRepository()
IProductRepository CreateProductRepository()
IOrderRepository CreateOrderRepository()

// Fresh instance creation
IUserRepository CreateNewUserRepository()
IProductRepository CreateNewProductRepository()
IOrderRepository CreateNewOrderRepository()

// Utility
void ResetRepositories()
```

---

### 5. **ServiceFactory** ✅ (Singleton)
**Location:** `Factory\ServiceFactory.cs`

**Purpose:** Centralized service creation with dependency injection

**Features:**
- ✅ Singleton pattern for factory itself
- ✅ Cached service instances
- ✅ Automatic dependency injection
- ✅ Thread-safe implementation

**Methods:**
```csharp
// Singleton access
ServiceFactory.Instance

// Cached service creation (recommended)
UserService CreateUserService()
ProductService CreateProductService()
OrderService CreateOrderService()

// Fresh instance creation
UserService CreateNewUserService()
ProductService CreateNewProductService()
OrderService CreateNewOrderService()

// Utility
void ResetServices()
```

---

## 🔄 Integration with Existing Patterns

### Services Now Use Factories

**UserService** - Uses `UserFactory`
```csharp
public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserFactory _userFactory;  // ✨ Factory injected
    
    public bool AddUser(string username, string password, string role, ...)
    {
        // ✅ Use factory to create user
        var user = _userFactory.CreateUser(username, password, role);
        // ... repository saves it
    }
}
```

**ProductService** - Uses `ProductFactory`
```csharp
public class ProductService
{
    private readonly IProductRepository _repo;
    private readonly IProductFactory _productFactory;  // ✨ Factory injected
    
    public bool AddProduct(string name, int categoryId, ...)
    {
        // ✅ Use factory to create product
        var product = _productFactory.CreateProduct(name, categoryId, ...);
        // ... repository saves it
    }
}
```

**OrderService** - Uses `OrderFactory`
```csharp
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderFactory _orderFactory;  // ✨ Factory injected
    
    public bool AddOrder(int userId, decimal totalAmount, ...)
    {
        // ✅ Use factory to create order
        var order = _orderFactory.CreatePendingOrder(userId, totalAmount, ...);
        // ... repository saves it
    }
    
    // ✨ New specialized methods
    public bool CreateDineInOrder(int userId, int tableNumber, ...)
    public bool CreateTakeawayOrder(int userId, string customerName, ...)
    public bool CreateDeliveryOrder(int userId, string deliveryAddress, ...)
}
```

---

## 🎨 UI Layer Updates

### AdminAddUser.cs
**BEFORE** ❌
```csharp
public AdminAddUser()
{
    _userService = new UserService(new UserRepository());
}
```

**AFTER** ✅
```csharp
public AdminAddUser()
{
    // Use ServiceFactory (Factory Pattern)
    _userService = ServiceFactory.Instance.CreateUserService();
}
```

### AdminAddProducts.cs
**BEFORE** ❌
```csharp
public AdminAddProducts()
{
    var repo = new ProductRepository();
    _productService = new ProductService(repo);
}
```

**AFTER** ✅
```csharp
public AdminAddProducts()
{
    // Use ServiceFactory (Factory Pattern)
    _productService = ServiceFactory.Instance.CreateProductService();
}
```

### POSForm.cs
**BEFORE** ❌
```csharp
public POSForm()
{
    _productService = new ProductService(new ProductRepository());
}
```

**AFTER** ✅
```csharp
public POSForm()
{
    // Use ServiceFactory (Factory Pattern)
    _productService = ServiceFactory.Instance.CreateProductService();
}
```

---

## 📊 Complete Architecture

```
┌────────────────────────────────────────────┐
│         UI Layer (Views)                   │
│  - AdminAddUser.cs                         │
│  - AdminAddProducts.cs                     │
│  - POSForm.cs                              │
└──────────────┬─────────────────────────────┘
               │ uses ServiceFactory
               ▼
┌────────────────────────────────────────────┐
│      ServiceFactory (Singleton)            │
│  Creates → UserService                     │
│          → ProductService                  │
│          → OrderService                    │
└──────────────┬─────────────────────────────┘
               │ uses
               ▼
┌────────────────────────────────────────────┐
│      Service Layer                         │
│  - UserService    → Uses UserFactory       │
│  - ProductService → Uses ProductFactory    │
│  - OrderService   → Uses OrderFactory      │
└──────────────┬─────────────────────────────┘
               │ uses RepositoryFactory
               ▼
┌────────────────────────────────────────────┐
│    RepositoryFactory (Singleton)           │
│  Creates → UserRepository                  │
│          → ProductRepository               │
│          → OrderRepository                 │
└──────────────┬─────────────────────────────┘
               │ uses
               ▼
┌────────────────────────────────────────────┐
│    Repository Layer                        │
│  - UserRepository                          │
│  - ProductRepository                       │
│  - OrderRepository                         │
└──────────────┬─────────────────────────────┘
               │ uses
               ▼
┌────────────────────────────────────────────┐
│    DbConnection (Singleton)                │
└────────────────────────────────────────────┘
```

---

## 💡 Usage Examples

### 1. Creating Users

**Without Factory (Old Way)** ❌
```csharp
var user = new Users
{
    Username = "john_doe",
    Password = "password123",
    Role = "cashier",
    Status = "Active",
    RegisterDate = DateTime.Now
};
```

**With Factory (New Way)** ✅
```csharp
var factory = new UserFactory();

// Generic method
var user = factory.CreateUser("john_doe", "password123", "cashier");

// Or specialized method
var cashier = factory.CreateCashier("john_doe", "password123");
var admin = factory.CreateAdmin("admin_user", "admin123");
```

---

### 2. Creating Orders

**Without Factory (Old Way)** ❌
```csharp
var order = new Order
{
    UserID = userId,
    Status = "Pending",
    OrderDate = DateTime.Now,
    TotalAmount = 25.50m,
    Discount = 2.00m,
    Note = "Dine-in | Table: 5"
};
```

**With Factory (New Way)** ✅
```csharp
var factory = new OrderFactory();

// Dine-in order
var dineInOrder = factory.CreateDineInOrder(userId, tableNumber: 5);

// Takeaway order
var takeawayOrder = factory.CreateTakeawayOrder(userId, "John Smith");

// Delivery order
var deliveryOrder = factory.CreateDeliveryOrder(
    userId,
    deliveryAddress: "123 Main St",
    customerName: "Jane Doe",
    phone: "555-1234"
);

// Standard pending order
var order = factory.CreatePendingOrder(userId, totalAmount: 25.50m, discount: 2.00m);
```

---

### 3. Creating Products

**Without Factory (Old Way)** ❌
```csharp
var product = new Products
{
    Name = "Espresso",
    CategoryID = 1,
    SupplierID = 2,
    Price = 3.50m,
    Discount = 0.25m,
    Status = "active"
};
product.Validate(); // Manual validation
```

**With Factory (New Way)** ✅
```csharp
var factory = new ProductFactory();

// Standard product (validation automatic)
var product = factory.CreateProduct("Espresso", categoryId: 1, supplierId: 2, price: 3.50m, discount: 0.25m);

// Beverage (category auto-assigned)
var coffee = factory.CreateBeverageProduct("Latte", supplierId: 2, price: 4.50m);

// Food product
var sandwich = factory.CreateFoodProduct("Club Sandwich", supplierId: 3, price: 8.99m);

// Promotional product (with percentage discount)
var promo = factory.CreatePromotionalProduct("Summer Special", categoryId: 1, supplierId: 2, 
    price: 10.00m, discountPercent: 25.0m);
```

---

### 4. Using ServiceFactory in UI

**Simple Usage** ✅
```csharp
public class MyUserControl : UserControl
{
    private UserService _userService;
    private ProductService _productService;
    private OrderService _orderService;
    
    public MyUserControl()
    {
        InitializeComponent();
        
        // Get all services from factory
        _userService = ServiceFactory.Instance.CreateUserService();
        _productService = ServiceFactory.Instance.CreateProductService();
        _orderService = ServiceFactory.Instance.CreateOrderService();
    }
}
```

---

### 5. Complete Example: Adding a User

```csharp
// In your UI code
var serviceFactory = ServiceFactory.Instance;
var userService = serviceFactory.CreateUserService();

try
{
    // UserService internally uses UserFactory
    bool success = userService.AddUser(
        username: "new_cashier",
        password: "secure123",
        role: "cashier",
        status: "Active"
    );
    
    if (success)
    {
        MessageBox.Show("User created successfully!");
    }
}
catch (Exception ex)
{
    MessageBox.Show($"Error: {ex.Message}");
}
```

---

### 6. Complete Example: Creating Orders

```csharp
// In POSForm or OrderManagement
var orderService = ServiceFactory.Instance.CreateOrderService();
int userId = UserSession.UserId;

// Scenario 1: Dine-in order
bool success = orderService.CreateDineInOrder(
    userId: userId,
    tableNumber: 12,
    totalAmount: 45.50m,
    discount: 5.00m
);

// Scenario 2: Takeaway order
success = orderService.CreateTakeawayOrder(
    userId: userId,
    customerName: "John Doe",
    totalAmount: 25.00m,
    discount: 0
);

// Scenario 3: Delivery order
success = orderService.CreateDeliveryOrder(
    userId: userId,
    deliveryAddress: "456 Elm Street, Apt 3B",
    customerName: "Jane Smith",
    phone: "555-9876",
    totalAmount: 38.75m,
    discount: 3.00m
);
```

---

## 🎯 Benefits Achieved

### 1. **Centralized Object Creation**
- ✅ Single place to modify how objects are created
- ✅ Consistent initialization across the application
- ✅ Easier to maintain and test

### 2. **Reduced Code Duplication**
**Before:**
```csharp
// Scattered in multiple places
var user = new Users { Username = ..., Password = ..., Role = ..., Status = "Active", RegisterDate = DateTime.Now };
```

**After:**
```csharp
// Centralized in factory
var user = userFactory.CreateCashier(username, password);
```

### 3. **Built-in Validation**
```csharp
// Factory validates parameters automatically
var product = productFactory.CreateProduct(...);
// Throws ArgumentException if invalid
```

### 4. **Better Testing**
```csharp
// Easy to mock factories in tests
var mockFactory = new Mock<IUserFactory>();
mockFactory.Setup(f => f.CreateUser(...)).Returns(testUser);

var service = new UserService(mockRepo, mockFactory.Object);
```

### 5. **Type Safety**
```csharp
// Specialized methods ensure correct object creation
var admin = userFactory.CreateAdmin(...);    // Always creates admin
var cashier = userFactory.CreateCashier(...); // Always creates cashier
```

---

## 📈 Patterns Summary

| Pattern | Status | Components |
|---------|--------|------------|
| **Singleton** | ✅ Complete | DbConnection, RepositoryFactory, ServiceFactory |
| **Repository** | ✅ Complete | User, Product, Order repositories |
| **Factory** | ✅ Complete | User, Product, Order, Repository, Service factories |
| **Strategy** | ⏳ Next | Discount strategies, Payment strategies |
| **State** | ⏳ Pending | Order state management |
| **Observer** | ⏳ Pending | Real-time updates |

---

## 🔍 Factory Pattern Types Implemented

### 1. **Simple Factory** - UserFactory, ProductFactory, OrderFactory
Creates objects based on parameters

### 2. **Singleton Factory** - RepositoryFactory, ServiceFactory
Combines Factory + Singleton for centralized creation

### 3. **Factory Method Pattern**
Each factory has specialized creation methods:
- `CreateAdmin()`, `CreateCashier()` in UserFactory
- `CreateBeverageProduct()`, `CreateFoodProduct()` in ProductFactory
- `CreateDineInOrder()`, `CreateDeliveryOrder()` in OrderFactory

---

## ✅ Build Status: **SUCCESS** ✅

All files compile without errors!

---

## 🚀 Next Steps

**Pattern #3: Strategy Pattern**
- Implement discount strategies (Percentage, Fixed Amount, BOGO)
- Implement payment strategies (Cash, Card, E-wallet)

**Would you like to proceed?**

---

**Factory Pattern Implementation** ✅ **COMPLETED**
**Total Patterns Implemented:** 2/6
- ✅ Singleton Pattern
- ✅ Factory Pattern
- ⏳ Strategy Pattern (Next)
- ⏳ State Pattern
- ⏳ Observer Pattern
- ⏳ Additional patterns as needed
