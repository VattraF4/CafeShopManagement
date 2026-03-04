using System;
using OOADCafeShopManagement.Models;

namespace OOADCafeShopManagement.State
{
    /// <summary>
    /// State Pattern for Order state management
    /// Manages order lifecycle and valid state transitions
    /// </summary>
    public interface IOrderState
    {
        void Process(OrderContext context);
        void Complete(OrderContext context);
        void Cancel(OrderContext context);
        void Refund(OrderContext context);
        string GetStateName();
        bool CanTransitionTo(string targetState);
    }

    /// <summary>
    /// Context that maintains current state
    /// </summary>
    public class OrderContext
    {
        public Order Order { get; set; }
        public IOrderState CurrentState { get; set; }
        public DateTime LastStateChange { get; set; }
        public string StateChangeReason { get; set; }

        public OrderContext(Order order)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
            
            // Set initial state based on order status
            switch (order.Status?.ToLower())
            {
                case "pending":
                    CurrentState = new PendingState();
                    break;
                case "processing":
                    CurrentState = new ProcessingState();
                    break;
                case "completed":
                    CurrentState = new CompletedState();
                    break;
                case "cancelled":
                    CurrentState = new CancelledState();
                    break;
                case "refunded":
                    CurrentState = new RefundedState();
                    break;
                default:
                    CurrentState = new PendingState();
                    break;
            }
            
            LastStateChange = DateTime.Now;
        }

        public void SetState(IOrderState newState, string reason = null)
        {
            if (newState == null)
                throw new ArgumentNullException(nameof(newState));

            CurrentState = newState;
            Order.Status = newState.GetStateName();
            LastStateChange = DateTime.Now;
            StateChangeReason = reason;
        }

        public void Process()
        {
            CurrentState.Process(this);
        }

        public void Complete()
        {
            CurrentState.Complete(this);
        }

        public void Cancel()
        {
            CurrentState.Cancel(this);
        }

        public void Refund()
        {
            CurrentState.Refund(this);
        }

        public string GetCurrentStateName()
        {
            return CurrentState.GetStateName();
        }
    }

    /// <summary>
    /// Pending State - Order created but not yet processed
    /// </summary>
    public class PendingState : IOrderState
    {
        public void Process(OrderContext context)
        {
            // Transition to Processing state
            context.SetState(new ProcessingState(), "Order is being prepared");
        }

        public void Complete(OrderContext context)
        {
            // Can't complete from pending - must process first
            throw new InvalidOperationException("Cannot complete order from Pending state. Process the order first.");
        }

        public void Cancel(OrderContext context)
        {
            // Can cancel from pending
            context.SetState(new CancelledState(), "Order cancelled by user");
        }

        public void Refund(OrderContext context)
        {
            // Can't refund pending order
            throw new InvalidOperationException("Cannot refund order from Pending state.");
        }

        public string GetStateName()
        {
            return "Pending";
        }

        public bool CanTransitionTo(string targetState)
        {
            return targetState == "Processing" || targetState == "Cancelled";
        }
    }

    /// <summary>
    /// Processing State - Order is being prepared
    /// </summary>
    public class ProcessingState : IOrderState
    {
        public void Process(OrderContext context)
        {
            // Already processing
            throw new InvalidOperationException("Order is already being processed.");
        }

        public void Complete(OrderContext context)
        {
            // Transition to Completed state
            context.SetState(new CompletedState(), "Order completed successfully");
        }

        public void Cancel(OrderContext context)
        {
            // Can cancel from processing (might charge cancellation fee)
            context.SetState(new CancelledState(), "Order cancelled during processing");
        }

        public void Refund(OrderContext context)
        {
            // Can't refund while processing
            throw new InvalidOperationException("Cannot refund order while it's being processed. Cancel it first.");
        }

        public string GetStateName()
        {
            return "Processing";
        }

        public bool CanTransitionTo(string targetState)
        {
            return targetState == "Completed" || targetState == "Cancelled";
        }
    }

    /// <summary>
    /// Completed State - Order has been completed
    /// </summary>
    public class CompletedState : IOrderState
    {
        public void Process(OrderContext context)
        {
            // Can't process completed order
            throw new InvalidOperationException("Cannot process an already completed order.");
        }

        public void Complete(OrderContext context)
        {
            // Already completed
            throw new InvalidOperationException("Order is already completed.");
        }

        public void Cancel(OrderContext context)
        {
            // Can't cancel completed order - must refund
            throw new InvalidOperationException("Cannot cancel completed order. Use refund instead.");
        }

        public void Refund(OrderContext context)
        {
            // Can refund completed order
            context.SetState(new RefundedState(), "Order refunded");
        }

        public string GetStateName()
        {
            return "Completed";
        }

        public bool CanTransitionTo(string targetState)
        {
            return targetState == "Refunded";
        }
    }

    /// <summary>
    /// Cancelled State - Order has been cancelled
    /// </summary>
    public class CancelledState : IOrderState
    {
        public void Process(OrderContext context)
        {
            // Can't process cancelled order
            throw new InvalidOperationException("Cannot process a cancelled order.");
        }

        public void Complete(OrderContext context)
        {
            // Can't complete cancelled order
            throw new InvalidOperationException("Cannot complete a cancelled order.");
        }

        public void Cancel(OrderContext context)
        {
            // Already cancelled
            throw new InvalidOperationException("Order is already cancelled.");
        }

        public void Refund(OrderContext context)
        {
            // Cancelled orders don't need refund (payment not processed)
            throw new InvalidOperationException("Cancelled orders cannot be refunded.");
        }

        public string GetStateName()
        {
            return "Cancelled";
        }

        public bool CanTransitionTo(string targetState)
        {
            // Terminal state - no transitions allowed
            return false;
        }
    }

    /// <summary>
    /// Refunded State - Order has been refunded
    /// </summary>
    public class RefundedState : IOrderState
    {
        public void Process(OrderContext context)
        {
            throw new InvalidOperationException("Cannot process a refunded order.");
        }

        public void Complete(OrderContext context)
        {
            throw new InvalidOperationException("Cannot complete a refunded order.");
        }

        public void Cancel(OrderContext context)
        {
            throw new InvalidOperationException("Cannot cancel a refunded order.");
        }

        public void Refund(OrderContext context)
        {
            throw new InvalidOperationException("Order is already refunded.");
        }

        public string GetStateName()
        {
            return "Refunded";
        }

        public bool CanTransitionTo(string targetState)
        {
            // Terminal state - no transitions allowed
            return false;
        }
    }
}
