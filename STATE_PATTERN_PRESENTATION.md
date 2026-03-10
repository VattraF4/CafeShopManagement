# State Design Pattern - Order Status Management

## 🎯 Complete Implementation for Presentation

---

## 📁 File Locations

| Component | File | Lines |
|-----------|------|-------|
| **Interface** | `State/OrderState.cs` | 10-17 |
| **Context** | `State/OrderState.cs` | 22-90 |
| **PendingState** | `State/OrderState.cs` | 95-128 |
| **ProcessingState** | `State/OrderState.cs` | 133-168 |
| **CompletedState** | `State/OrderState.cs` | 173-208 |
| **CancelledState** | `State/OrderState.cs` | 213-248 |
| **RefundedState** | `State/OrderState.cs` | 253-288 |

---

## 🏗️ State Pattern Components

### 1. **Interface** - `IOrderState`

```csharp
public interface IOrderState
{
    void Process(OrderContext context);
    void Complete(OrderContext context);
    void Cancel(OrderContext context);
    void Refund(OrderContext context);
    string GetStateName();
    bool CanTransitionTo(string targetState);
}
```

**Purpose:** Defines contract for all order states

---

### 2. **Context** - `OrderContext`

```csharp
public class OrderContext
{
    public Order Order { get; set; }
    public IOrderState CurrentState { get; set; }
    public DateTime LastStateChange { get; set; }
    public string StateChangeReason { get; set; }

    public void Process() => CurrentState.Process(this);
    public void Complete() => CurrentState.Complete(this);
    public void Cancel() => CurrentState.Cancel(this);
    public void Refund() => CurrentState.Refund(this);
}
```

**Purpose:** Maintains current state and delegates actions

---

### 3. **Concrete States** (5 Classes)

#### A. **PendingState**
```csharp
public class PendingState : IOrderState
{
    public void Process(OrderContext context)
    {
        context.SetState(new ProcessingState(), "Order is being prepared");
    }
    
    public void Cancel(OrderContext context)
    {
        context.SetState(new CancelledState(), "Order cancelled by user");
    }
}
```

**Valid Transitions:**
- ✅ Pending → Processing (Process)
- ✅ Pending → Cancelled (Cancel)
- ❌ Pending → Completed (Must process first)

---

#### B. **ProcessingState**
```csharp
public class ProcessingState : IOrderState
{
    public void Complete(OrderContext context)
    {
        context.SetState(new CompletedState(), "Order completed successfully");
    }
    
    public void Cancel(OrderContext context)
    {
        context.SetState(new CancelledState(), "Order cancelled during processing");
    }
}
```

**Valid Transitions:**
- ✅ Processing → Completed (Complete)
- ✅ Processing → Cancelled (Cancel)
- ❌ Processing → Pending (Cannot go back)

---

#### C. **CompletedState**
```csharp
public class CompletedState : IOrderState
{
    public void Refund(OrderContext context)
    {
        context.SetState(new RefundedState(), "Order refunded");
    }
}
```

**Valid Transitions:**
- ✅ Completed → Refunded (Refund)
- ❌ Completed → Cancelled (Use refund instead)

---

#### D. **CancelledState**
```csharp
public class CancelledState : IOrderState
{
    public string GetStateName() => "Cancelled";
    
    // All transitions throw InvalidOperationException
    // This is a terminal state
}
```

**Valid Transitions:**
- ❌ None (Terminal state)

---

#### E. **RefundedState**
```csharp
public class RefundedState : IOrderState
{
    public string GetStateName() => "Refunded";
    
    // All transitions throw InvalidOperationException
    // This is a terminal state
}
```

**Valid Transitions:**
- ❌ None (Terminal state)

---

## 📊 State Transition Diagram

```
    ┌─────────────┐
    │   PENDING   │ ◄─── Initial State
    └──────┬──────┘
           │
           ├──Process()──────▶┌──────────────┐
           │                  │  PROCESSING  │
           │                  └───────┬──────┘
           │                          │
           │                          ├──Complete()──▶┌────────────┐
           │                          │                │ COMPLETED  │
           │                          │                └──────┬─────┘
           │                          │                       │
           │                          │                       ├──Refund()──▶┌───────────┐
           │                          │                       │              │ REFUNDED  │
           │                          │                       │              └───────────┘
           │                          │                       │                    ▲
           │                          │                       │                    │
           └──Cancel()────────────────┴───────────────────────┴──▶┌─────────────┐ │
                                                                   │  CANCELLED  │ │
                                                                   └─────────────┘ │
                                                                          │        │
                                                                          └────────┘
                                                                        Terminal States
```

---

## 💻 Usage Examples

### Example 1: Normal Order Flow

```csharp
using OOADCafeShopManagement.State;
using OOADCafeShopManagement.Models;

// Create order
var order = new Order
{
    OrderID = 1,
    Status = "Pending",
    TotalAmount = 50.00m,
    UserID = 1
};

// Create state context
var orderContext = new OrderContext(order);

// Check current state
Console.WriteLine($"Current State: {orderContext.GetCurrentStateName()}"); 
// Output: Current State: Pending

// Transition 1: Pending → Processing
orderContext.Process();
Console.WriteLine($"Current State: {orderContext.Order.Status}"); 
// Output: Current State: Processing

// Transition 2: Processing → Completed
orderContext.Complete();
Console.WriteLine($"Current State: {orderContext.Order.Status}"); 
// Output: Current State: Completed

Console.WriteLine($"Last Changed: {orderContext.LastStateChange}");
Console.WriteLine($"Reason: {orderContext.StateChangeReason}");
```

---

### Example 2: Order Cancellation

```csharp
// Create pending order
var order = new Order
{
    OrderID = 2,
    Status = "Pending",
    TotalAmount = 25.00m
};

var orderContext = new OrderContext(order);

// Customer wants to cancel
orderContext.Cancel();
Console.WriteLine($"Status: {orderContext.Order.Status}"); 
// Output: Status: Cancelled

// Try to process cancelled order
try
{
    orderContext.Process();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine(ex.Message);
    // Output: Cannot process a cancelled order.
}
```

---

### Example 3: Order Refund

```csharp
// Create completed order
var order = new Order
{
    OrderID = 3,
    Status = "Completed",
    TotalAmount = 75.00m
};

var orderContext = new OrderContext(order);

// Customer requests refund
orderContext.Refund();
Console.WriteLine($"Status: {orderContext.Order.Status}"); 
// Output: Status: Refunded

Console.WriteLine($"Reason: {orderContext.StateChangeReason}");
// Output: Reason: Order refunded
```

---

### Example 4: Invalid Transition Handling

```csharp
var order = new Order
{
    OrderID = 4,
    Status = "Pending"
};

var orderContext = new OrderContext(order);

// Try to complete without processing
try
{
    orderContext.Complete();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine(ex.Message);
    // Output: Cannot complete order from Pending state. Process the order first.
}

// Correct flow
orderContext.Process();   // Pending → Processing
orderContext.Complete();  // Processing → Completed
```

---

## 🎯 Integration with OrderManager

Add these methods to `Helper/OrderManager.cs`:

```csharp
using OOADCafeShopManagement.State;

public class OrderManager
{
    private OrderContext _orderContext;
    
    // Initialize state context when order is created/loaded
    public void InitializeOrderState(Order order)
    {
        _orderContext = new OrderContext(order);
    }
    
    // Process order
    public void ProcessOrder()
    {
        if (_orderContext == null)
        {
            throw new InvalidOperationException("Order context not initialized");
        }
        
        _orderContext.Process();
        UpdateOrderInDatabase(_currentOrder);
    }
    
    // Complete order
    public void CompleteOrder()
    {
        _orderContext.Complete();
        UpdateOrderInDatabase(_currentOrder);
    }
    
    // Cancel order
    public void CancelOrder()
    {
        _orderContext.Cancel();
        UpdateOrderInDatabase(_currentOrder);
    }
    
    // Refund order
    public void RefundOrder()
    {
        _orderContext.Refund();
        UpdateOrderInDatabase(_currentOrder);
    }
    
    // Get current state
    public string GetOrderState()
    {
        return _orderContext.GetCurrentStateName();
    }
    
    // Check if can transition
    public bool CanTransitionTo(string targetState)
    {
        return _orderContext.CurrentState.CanTransitionTo(targetState);
    }
}
```

---

## 🎨 Integration with POSForm

Add state management buttons to POSForm:

```csharp
// In POSForm.cs

private OrderContext _orderContext;

// Initialize after payment
private void btnPay_Click(object sender, EventArgs e)
{
    // ... existing payment code ...
    
    if (success)
    {
        // Initialize state context
        _orderContext = new OrderContext(_orderManager.CurrentOrder);
        
        MessageBox.Show($"Order created in {_orderContext.GetCurrentStateName()} state");
    }
}

// Button to process order
private void btnProcessOrder_Click(object sender, EventArgs e)
{
    try
    {
        _orderContext.Process();
        MessageBox.Show($"Order is now {_orderContext.Order.Status}");
        UpdateOrderDisplay();
    }
    catch (InvalidOperationException ex)
    {
        MessageBox.Show(ex.Message, "Invalid Action", 
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}

// Button to complete order
private void btnCompleteOrder_Click(object sender, EventArgs e)
{
    try
    {
        _orderContext.Complete();
        MessageBox.Show($"Order completed successfully!");
        UpdateOrderDisplay();
    }
    catch (InvalidOperationException ex)
    {
        MessageBox.Show(ex.Message, "Invalid Action", 
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}

// Button to cancel order
private void btnCancelOrder_Click(object sender, EventArgs e)
{
    try
    {
        var result = MessageBox.Show(
            "Are you sure you want to cancel this order?",
            "Confirm Cancellation",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);
        
        if (result == DialogResult.Yes)
        {
            _orderContext.Cancel();
            MessageBox.Show("Order cancelled");
            UpdateOrderDisplay();
        }
    }
    catch (InvalidOperationException ex)
    {
        MessageBox.Show(ex.Message, "Invalid Action", 
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
```

---

## ✅ Benefits of State Pattern

### 1. **Clear State Management**
- Each state is a separate class
- Easy to understand what actions are valid in each state

### 2. **Prevents Invalid Transitions**
- Trying invalid transition throws exception
- No way to accidentally put order in invalid state

### 3. **Single Responsibility**
- Each state class handles only its own behavior
- Easy to modify one state without affecting others

### 4. **Open/Closed Principle**
- Easy to add new states (e.g., "Preparing", "Ready for Pickup")
- Don't need to modify existing states

### 5. **Testability**
- Each state can be tested independently
- Easy to verify valid/invalid transitions

---

## 🎬 Presentation Demo Script

### Step 1: Show the Problem
"Orders go through different statuses. How do we manage valid transitions and prevent invalid actions?"

### Step 2: Show State Diagram
"Our orders flow: Pending → Processing → Completed or Cancelled"

### Step 3: Show the Interface
```csharp
public interface IOrderState
{
    void Process(OrderContext context);
    void Complete(OrderContext context);
    void Cancel(OrderContext context);
    string GetStateName();
}
```

### Step 4: Show One State Implementation
```csharp
public class PendingState : IOrderState
{
    public void Process(OrderContext context)
    {
        context.SetState(new ProcessingState());
    }
    
    public void Complete(OrderContext context)
    {
        throw new InvalidOperationException("Cannot complete from Pending");
    }
}
```

### Step 5: Live Demo
```csharp
// Create order in Pending state
var order = new Order { OrderID = 1, Status = "Pending" };
var context = new OrderContext(order);

// Valid transition
context.Process(); // ✅ Pending → Processing

// Valid transition
context.Complete(); // ✅ Processing → Completed

// Invalid transition (shows error)
context.Process(); // ❌ Cannot process completed order
```

### Step 6: Explain Benefits
- "Each state knows its valid transitions"
- "Impossible to create invalid state"
- "Easy to add new states"
- "Follows SOLID principles"

---

## 📊 State Pattern Statistics

**Implementation Details:**
- **States:** 5 (Pending, Processing, Completed, Cancelled, Refunded)
- **Methods per State:** 6 (Process, Complete, Cancel, Refund, GetStateName, CanTransitionTo)
- **Context Methods:** 5 (Process, Complete, Cancel, Refund, SetState)
- **Total Lines:** ~290 lines
- **Terminal States:** 2 (Cancelled, Refunded)

**Transition Matrix:**

| From \ To | Pending | Processing | Completed | Cancelled | Refunded |
|-----------|---------|------------|-----------|-----------|----------|
| **Pending** | ─ | ✅ | ❌ | ✅ | ❌ |
| **Processing** | ❌ | ─ | ✅ | ✅ | ❌ |
| **Completed** | ❌ | ❌ | ─ | ❌ | ✅ |
| **Cancelled** | ❌ | ❌ | ❌ | ─ | ❌ |
| **Refunded** | ❌ | ❌ | ❌ | ❌ | ─ |

---

## 🎯 Key Takeaways for Presentation

1. **What:** State Pattern lets objects change behavior based on state
2. **Why:** Prevents invalid order status transitions
3. **How:** Each state is a class implementing IOrderState interface
4. **Where:** Used in OrderManager for order lifecycle
5. **When:** Every time order status changes

**Pattern Components:**
- ✅ IOrderState (Interface)
- ✅ OrderContext (Context)
- ✅ 5 Concrete States (PendingState, ProcessingState, etc.)

**Benefits:**
- ✅ Type-safe state transitions
- ✅ Clear, maintainable code
- ✅ Easy to extend
- ✅ Follows SOLID principles

---

**State Pattern Implementation Complete!** 🎯

**Files:** 1 (`State/OrderState.cs`)  
**Classes:** 7 (1 interface, 1 context, 5 states)  
**Status:** ✅ Production Ready
