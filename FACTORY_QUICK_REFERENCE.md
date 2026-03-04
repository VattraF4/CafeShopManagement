# Factory Pattern - Quick Reference Guide

## 🚀 Quick Start

### Get Services (Recommended Way)
```csharp
// In any UI component
using OOADCafeShopManagement.Factory;

var userService = ServiceFactory.Instance.CreateUserService();
var productService = ServiceFactory.Instance.CreateProductService();
var orderService = ServiceFactory.Instance.CreateOrderService();
```

---

## 📦 Factory Methods Cheat Sheet

### UserFactory

```csharp
var factory = new UserFactory();

// By role (generic)
var user = factory.CreateUser("username", "password", "admin");

// Specialized
var admin = factory.CreateAdmin("admin_user", "admin123");
var cashier = factory.CreateCashier("cashier_user", "pass123");
var manager = factory.CreateManager("manager_user", "pass123");
var staff = factory.CreateStaff("staff_user", "pass123");
```

### OrderFactory

```csharp
var factory = new OrderFactory();

// Standard orders
var order = factory.CreatePendingOrder(userId, totalAmount: 50.00m, discount: 5.00m);
var completed = factory.CreateCompletedOrder(userId, totalAmount: 30.00m);

// Specialized orders
var dineIn = factory.CreateDineInOrder(userId, tableNumber: 5);
var takeaway = factory.CreateTakeawayOrder(userId, "John Doe");
var delivery = factory.CreateDeliveryOrder(userId, "123 Main St", "Jane", "555-1234");
```

### ProductFactory

```csharp
var factory = new ProductFactory();

// Standard product
var product = factory.CreateProduct("Coffee", categoryId: 1, supplierId: 2, price: 5.00m);

// By category
var beverage = factory.CreateBeverageProduct("Latte", supplierId: 2, price: 4.50m);
var food = factory.CreateFoodProduct("Sandwich", supplierId: 3, price: 8.99m);
var dessert = factory.CreateDessertProduct("Cake", supplierId: 4, price: 6.50m);

// Special offers
var promo = factory.CreatePromotionalProduct("Summer Special", 1, 2, 10.00m, discountPercent: 25.0m);
var discounted = factory.CreateProductWithPercentDiscount("Winter Deal", 1, 2, 8.00m, 15.0m);
```

---

## 🎯 Common Scenarios

### Scenario 1: Add New User
```csharp
var userService = ServiceFactory.Instance.CreateUserService();

bool success = userService.AddUser(
    username: "new_employee",
    password: "secure_password",
    role: "cashier",  // Factory handles role validation
    status: "Active"
);
```

### Scenario 2: Create Dine-in Order
```csharp
var orderService = ServiceFactory.Instance.CreateOrderService();

bool success = orderService.CreateDineInOrder(
    userId: UserSession.UserId,
    tableNumber: 8,
    totalAmount: 45.50m,
    discount: 5.00m
);
```

### Scenario 3: Add Product with Promotion
```csharp
var productFactory = new ProductFactory();
var productService = ServiceFactory.Instance.CreateProductService();

// Create promotional product (20% off)
var product = productFactory.CreatePromotionalProduct(
    name: "Holiday Special Latte",
    categoryId: 1,
    supplierId: 2,
    price: 6.00m,
    discountPercent: 20.0m
);

// Add to database (service handles the rest)
productService.AddProduct(product.Name, product.CategoryID, product.SupplierID, 
    product.Price, product.Discount);
```

---

## 💡 Best Practices

### ✅ DO
```csharp
// Use ServiceFactory in UI components
var service = ServiceFactory.Instance.CreateUserService();

// Use specialized factory methods
var admin = userFactory.CreateAdmin("username", "password");

// Let factories handle validation
var product = productFactory.CreateProduct(...); // Validation automatic
```

### ❌ DON'T
```csharp
// Don't create services manually
var service = new UserService(new UserRepository()); // ❌

// Don't create objects directly in business logic
var user = new Users { ... }; // ❌ Use factory instead

// Don't skip factory methods
var order = new Order { ... }; // ❌ Use factory instead
```

---

## 🔧 Advanced Usage

### Custom Factory Instance
```csharp
// If you need specific factory instance
var userFactory = new UserFactory();
var productFactory = new ProductFactory();
var orderFactory = new OrderFactory();

// Use in service constructor
var customService = new UserService(repository, userFactory);
```

### Reset Cached Instances
```csharp
// Clear cached services (useful for testing)
ServiceFactory.Instance.ResetServices();

// Clear cached repositories
RepositoryFactory.Instance.ResetRepositories();
```

### Create Fresh Instances
```csharp
// Get new instance each time (not cached)
var freshService = ServiceFactory.Instance.CreateNewUserService();
var freshRepo = RepositoryFactory.Instance.CreateNewUserRepository();
```

---

## 📊 Pattern Benefits Comparison

| Aspect | Without Factory | With Factory |
|--------|----------------|--------------|
| **Object Creation** | Scattered everywhere | Centralized |
| **Validation** | Manual, error-prone | Automatic |
| **Consistency** | Varies by developer | Guaranteed |
| **Maintenance** | Find all usages | Change one place |
| **Testing** | Hard to mock | Easy to mock |
| **Type Safety** | Weak | Strong |

---

## 🎓 Remember

1. **ServiceFactory** → Use in UI layer
2. **RepositoryFactory** → Used internally by ServiceFactory
3. **Entity Factories** (User/Product/Order) → Used internally by Services

**Flow:**
```
UI → ServiceFactory → Service → EntityFactory + Repository
```

---

## ✅ Checklist for New Features

When adding new functionality:

- [ ] Use `ServiceFactory.Instance.CreateXXXService()` in UI
- [ ] Use entity factories in service layer for object creation
- [ ] Don't instantiate models directly with `new`
- [ ] Don't create repositories/services manually
- [ ] Leverage specialized factory methods when available

---

**Happy Coding!** 🚀
