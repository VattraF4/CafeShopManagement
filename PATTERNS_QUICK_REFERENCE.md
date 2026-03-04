# Design Patterns - Quick Reference

## 🚀 Pattern Cheat Sheet

### Get Started Fast
```csharp
using OOADCafeShopManagement.Factory;
using OOADCafeShopManagement.Strategy;
using OOADCafeShopManagement.State;
using OOADCafeShopManagement.Observer;

// Get services (handles Singleton, Repository, Factory automatically)
var userService = ServiceFactory.Instance.CreateUserService();
var productService = ServiceFactory.Instance.CreateProductService();
var orderService = ServiceFactory.Instance.CreateOrderService();
```

---

## 1️⃣ Singleton Pattern - Get Single Instance

```csharp
// Database connection
var conn = DbConnection.Instance.GetConnection();

// Service factory
var serviceFactory = ServiceFactory.Instance;

// Repository factory
var repoFactory = RepositoryFactory.Instance;

// Order subject (for observers)
var orderSubject = OrderSubject.Instance;
```

---

## 2️⃣ Repository Pattern - Data Access

```csharp
// Through service (recommended)
var users = userService.GetAllUsers();
var products = productService.GetActiveProducts();
var orders = orderService.GetOrdersByUserId(userId);

// Direct repository (if needed)
var repo = RepositoryFactory.Instance.CreateUserRepository();
var user = repo.GetUserById(5);
```

---

## 3️⃣ Factory Pattern - Create Objects

```csharp
// Users
var userFactory = new UserFactory();
var admin = userFactory.CreateAdmin("admin", "pass");
var cashier = userFactory.CreateCashier("john", "pass");

// Products
var productFactory = new ProductFactory();
var coffee = productFactory.CreateBeverageProduct("Latte", 1, 4.50m);
var sandwich = productFactory.CreateFoodProduct("Club Sandwich", 2, 8.99m);

// Orders
var orderFactory = new OrderFactory();
var dineIn = orderFactory.CreateDineInOrder(userId, tableNumber: 5);
var delivery = orderFactory.CreateDeliveryOrder(userId, "123 Main St", "John", "555-1234");
```

---

## 4️⃣ Strategy Pattern - Swap Algorithms

### Discounts
```csharp
var payment = new PaymentContext();

// 15% off
payment.SetDiscountStrategy(new PercentageDiscountStrategy(15.0m));

// $5 off
payment.SetDiscountStrategy(new FixedAmountDiscountStrategy(5.00m));

// Buy One Get One
payment.SetDiscountStrategy(new BuyOneGetOneStrategy());

// Buy 2 Get 1 Free
payment.SetDiscountStrategy(new BuyXGetYFreeStrategy(2, 1));

// Seasonal (Holiday 30% off)
payment.SetDiscountStrategy(new SeasonalDiscountStrategy(
    30.0m, 
    new DateTime(2024, 12, 1), 
    new DateTime(2024, 12, 31), 
    "Holiday Sale"
));
```

### Payments
```csharp
// Cash
payment.SetPaymentStrategy(new CashPaymentStrategy());

// Card
payment.SetPaymentStrategy(new CardPaymentStrategy("4532123456789012", "John Doe"));

// E-Wallet
payment.SetPaymentStrategy(new EWalletPaymentStrategy("PayPal", "john@email.com"));

// Process
bool success = payment.ProcessPayment(amount: 50.00m, quantity: 2);
var receipt = payment.GetPaymentReceipt(amount: 50.00m, quantity: 2);
```

---

## 5️⃣ State Pattern - Order Lifecycle

```csharp
// Create state context
var order = new Order { OrderID = 1, Status = "Pending" };
var orderContext = new OrderContext(order);

// Transitions
orderContext.Process();   // Pending → Processing
orderContext.Complete();  // Processing → Completed
orderContext.Cancel();    // Pending/Processing → Cancelled
orderContext.Refund();    // Completed → Refunded

// Through service (recommended)
orderService.ProcessOrder(orderId);
orderService.CompleteOrder(orderId);
orderService.CancelOrder(orderId);
orderService.RefundOrder(orderId);
```

---

## 6️⃣ Observer Pattern - Event Notifications

```csharp
// Setup observers once
var subject = OrderSubject.Instance;
subject.Attach(new DashboardObserver());
subject.Attach(new NotificationObserver("Email"));
subject.Attach(new InventoryObserver());
subject.Attach(new AnalyticsObserver());
subject.Attach(new LoggingObserver());

// Now all order events automatically notify observers
orderService.AddOrder(userId, totalAmount);  // All observers notified
orderService.ProcessOrder(orderId);          // All observers notified
orderService.CompleteOrder(orderId);         // All observers notified
```

---

## 🎯 Common Scenarios

### Scenario 1: Add User
```csharp
var userService = ServiceFactory.Instance.CreateUserService();
userService.AddUser("john_doe", "password123", "cashier", "Active");
// Internally uses UserFactory automatically
```

### Scenario 2: Create Product with Promo
```csharp
var productFactory = new ProductFactory();
var product = productFactory.CreatePromotionalProduct(
    "Summer Special", 
    categoryId: 1, 
    supplierId: 2, 
    price: 10.00m, 
    discountPercent: 25.0m
);

var productService = ServiceFactory.Instance.CreateProductService();
productService.AddProduct(product.Name, product.CategoryID, 
    product.SupplierID, product.Price, product.Discount);
```

### Scenario 3: Process Order with Discount
```csharp
// 1. Setup payment & discount
var payment = new PaymentContext();
payment.SetDiscountStrategy(new PercentageDiscountStrategy(10.0m)); // 10% off
payment.SetPaymentStrategy(new CardPaymentStrategy("4532123456789012", "John"));

// 2. Process payment
bool paymentSuccess = payment.ProcessPayment(amount: 50.00m, quantity: 1);

if (paymentSuccess)
{
    // 3. Create & process order
    var orderService = ServiceFactory.Instance.CreateOrderService();
    orderService.CreateDineInOrder(userId: 1, tableNumber: 5, 
        totalAmount: 50.00m, discount: 5.00m);
    
    // 4. State transitions (observers notified automatically)
    orderService.ProcessOrder(orderId);
    orderService.CompleteOrder(orderId);
    
    // 5. Get receipt
    var receipt = payment.GetPaymentReceipt(50.00m, 1);
    Console.WriteLine(receipt);
}
```

### Scenario 4: Loyalty Discount with Points Payment
```csharp
var payment = new PaymentContext();

// Gold member gets 20% off
payment.SetDiscountStrategy(new LoyaltyDiscountStrategy("Gold Member", 20.0m));

// Pay with loyalty points (5000 points, 100 points = $1)
payment.SetPaymentStrategy(new PointsPaymentStrategy(points: 5000));

// Process
bool success = payment.ProcessPayment(25.00m, 2); // $50 - 20% = $40
var receipt = payment.GetPaymentReceipt(25.00m, 2);
```

---

## ⚡ Quick Tips

### ✅ DO
```csharp
// Use ServiceFactory in UI
var service = ServiceFactory.Instance.CreateUserService();

// Use factories for object creation
var user = userFactory.CreateAdmin("admin", "pass");

// Setup observers once at startup
OrderSubject.Instance.Attach(new DashboardObserver());

// Use state pattern for order transitions
orderService.ProcessOrder(orderId);
```

### ❌ DON'T
```csharp
// Don't create services manually
var service = new UserService(new UserRepository()); // ❌

// Don't create objects directly
var user = new Users { ... }; // ❌ Use factory

// Don't manually update order status
order.Status = "Completed"; // ❌ Use state pattern

// Don't forget to attach observers
// They won't get notifications if not attached
```

---

## 🔍 Debugging

### Check Pattern Usage
```csharp
// Verify singleton
var factory1 = ServiceFactory.Instance;
var factory2 = ServiceFactory.Instance;
Console.WriteLine(Object.ReferenceEquals(factory1, factory2)); // Should be true

// Check observer attachment
var subject = OrderSubject.Instance;
// subject should have observers attached

// Verify state transitions
try {
    orderContext.Complete(); // From Pending - should throw
} catch (InvalidOperationException ex) {
    Console.WriteLine("Correct! Must process first.");
}
```

---

## 📊 Pattern Checklist

Before deploying, verify:

- [ ] All UI uses ServiceFactory
- [ ] No direct `new Service()` or `new Repository()`
- [ ] Factories used for User/Product/Order creation
- [ ] Observers attached at application startup
- [ ] Order state transitions use State pattern
- [ ] Payment processing uses Strategy pattern
- [ ] Discount calculations use Strategy pattern

---

**Quick Start:** Copy-paste the "Complete Usage Example" from `ALL_PATTERNS_COMPLETE.md`!
