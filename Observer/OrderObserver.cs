using System;
using System.Collections.Generic;
using System.Linq;
using OOADCafeShopManagement.Models;

namespace OOADCafeShopManagement.Observer
{
    /// <summary>
    /// Observer Pattern for order events
    /// Allows multiple components to react to order changes
    /// </summary>
    public interface IOrderObserver
    {
        void OnOrderPlaced(Order order);
        void OnOrderStatusChanged(Order order, string oldStatus, string newStatus);
        void OnOrderCancelled(Order order);
        void OnOrderCompleted(Order order);
    }

    /// <summary>
    /// Subject (Observable) for order events
    /// </summary>
    public class OrderSubject
    {
        private readonly List<IOrderObserver> _observers = new List<IOrderObserver>();
        private static OrderSubject _instance;
        private static readonly object _lock = new object();

        private OrderSubject() { }

        public static OrderSubject Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new OrderSubject();
                        }
                    }
                }
                return _instance;
            }
        }

        public void Attach(IOrderObserver observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));
            
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        public void Detach(IOrderObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotifyOrderPlaced(Order order)
        {
            foreach (var observer in _observers)
            {
                observer.OnOrderPlaced(order);
            }
        }

        public void NotifyOrderStatusChanged(Order order, string oldStatus, string newStatus)
        {
            foreach (var observer in _observers)
            {
                observer.OnOrderStatusChanged(order, oldStatus, newStatus);
            }
        }

        public void NotifyOrderCancelled(Order order)
        {
            foreach (var observer in _observers)
            {
                observer.OnOrderCancelled(order);
            }
        }

        public void NotifyOrderCompleted(Order order)
        {
            foreach (var observer in _observers)
            {
                observer.OnOrderCompleted(order);
            }
        }
    }

    /// <summary>
    /// Dashboard Observer - Updates dashboard statistics
    /// </summary>
    public class DashboardObserver : IOrderObserver
    {
        private int _totalOrders = 0;
        private int _completedOrders = 0;
        private int _cancelledOrders = 0;
        private decimal _totalRevenue = 0;

        public void OnOrderPlaced(Order order)
        {
            _totalOrders++;
            System.Diagnostics.Debug.WriteLine($"[Dashboard] New order placed. Total orders: {_totalOrders}");
        }

        public void OnOrderStatusChanged(Order order, string oldStatus, string newStatus)
        {
            System.Diagnostics.Debug.WriteLine($"[Dashboard] Order {order.OrderID} status changed: {oldStatus} → {newStatus}");
        }

        public void OnOrderCancelled(Order order)
        {
            _cancelledOrders++;
            System.Diagnostics.Debug.WriteLine($"[Dashboard] Order cancelled. Total cancelled: {_cancelledOrders}");
        }

        public void OnOrderCompleted(Order order)
        {
            _completedOrders++;
            _totalRevenue += (order.TotalAmount - order.Discount);
            System.Diagnostics.Debug.WriteLine($"[Dashboard] Order completed. Revenue: ${_totalRevenue:F2}");
        }

        public int GetTotalOrders() => _totalOrders;
        public int GetCompletedOrders() => _completedOrders;
        public int GetCancelledOrders() => _cancelledOrders;
        public decimal GetTotalRevenue() => _totalRevenue;
    }

    /// <summary>
    /// Notification Observer - Sends notifications to users
    /// </summary>
    public class NotificationObserver : IOrderObserver
    {
        private readonly string _notificationChannel; // Email, SMS, Push, etc.

        public NotificationObserver(string channel = "Console")
        {
            _notificationChannel = channel;
        }

        public void OnOrderPlaced(Order order)
        {
            SendNotification($"Order #{order.OrderID} has been placed successfully.");
        }

        public void OnOrderStatusChanged(Order order, string oldStatus, string newStatus)
        {
            SendNotification($"Order #{order.OrderID} status updated: {oldStatus} → {newStatus}");
        }

        public void OnOrderCancelled(Order order)
        {
            SendNotification($"Order #{order.OrderID} has been cancelled.");
        }

        public void OnOrderCompleted(Order order)
        {
            SendNotification($"Order #{order.OrderID} has been completed. Thank you!");
        }

        private void SendNotification(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[{_notificationChannel}] {message}");
            
            // In real implementation:
            // - Send email
            // - Send SMS
            // - Send push notification
            // - Display toast message
        }
    }

    /// <summary>
    /// Inventory Observer - Updates inventory when orders are placed/completed
    /// </summary>
    public class InventoryObserver : IOrderObserver
    {
        public void OnOrderPlaced(Order order)
        {
            System.Diagnostics.Debug.WriteLine($"[Inventory] Reserving items for order #{order.OrderID}");
            // Reserve inventory items
        }

        public void OnOrderStatusChanged(Order order, string oldStatus, string newStatus)
        {
            if (newStatus == "Processing")
            {
                System.Diagnostics.Debug.WriteLine($"[Inventory] Confirming inventory for order #{order.OrderID}");
                // Confirm inventory deduction
            }
        }

        public void OnOrderCancelled(Order order)
        {
            System.Diagnostics.Debug.WriteLine($"[Inventory] Releasing reserved items for order #{order.OrderID}");
            // Release reserved inventory items
        }

        public void OnOrderCompleted(Order order)
        {
            System.Diagnostics.Debug.WriteLine($"[Inventory] Finalizing inventory for order #{order.OrderID}");
            // Finalize inventory changes
        }
    }

    /// <summary>
    /// Analytics Observer - Tracks order metrics and analytics
    /// </summary>
    public class AnalyticsObserver : IOrderObserver
    {
        private readonly Dictionary<string, int> _ordersByStatus = new Dictionary<string, int>();
        private readonly List<decimal> _orderAmounts = new List<decimal>();

        public void OnOrderPlaced(Order order)
        {
            _orderAmounts.Add(order.TotalAmount - order.Discount);
            System.Diagnostics.Debug.WriteLine($"[Analytics] Tracking new order. Average order value: ${GetAverageOrderValue():F2}");
        }

        public void OnOrderStatusChanged(Order order, string oldStatus, string newStatus)
        {
            // Track status changes
            if (!_ordersByStatus.ContainsKey(newStatus))
            {
                _ordersByStatus[newStatus] = 0;
            }
            _ordersByStatus[newStatus]++;
        }

        public void OnOrderCancelled(Order order)
        {
            System.Diagnostics.Debug.WriteLine($"[Analytics] Order cancelled. Cancellation rate: {GetCancellationRate():F2}%");
        }

        public void OnOrderCompleted(Order order)
        {
            System.Diagnostics.Debug.WriteLine($"[Analytics] Order completed. Completion rate: {GetCompletionRate():F2}%");
        }

        public decimal GetAverageOrderValue()
        {
            return _orderAmounts.Count > 0 ? _orderAmounts.Average() : 0;
        }

        public decimal GetCancellationRate()
        {
            int total = _ordersByStatus.Values.Sum();
            int cancelled = _ordersByStatus.ContainsKey("Cancelled") ? _ordersByStatus["Cancelled"] : 0;
            return total > 0 ? (decimal)cancelled / total * 100 : 0;
        }

        public decimal GetCompletionRate()
        {
            int total = _ordersByStatus.Values.Sum();
            int completed = _ordersByStatus.ContainsKey("Completed") ? _ordersByStatus["Completed"] : 0;
            return total > 0 ? (decimal)completed / total * 100 : 0;
        }
    }

    /// <summary>
    /// Logging Observer - Logs all order events
    /// </summary>
    public class LoggingObserver : IOrderObserver
    {
        private readonly string _logFilePath;

        public LoggingObserver(string logFilePath = null)
        {
            _logFilePath = logFilePath ?? "order_log.txt";
        }

        public void OnOrderPlaced(Order order)
        {
            Log($"ORDER_PLACED | OrderID: {order.OrderID} | Amount: ${order.TotalAmount:F2} | User: {order.UserID}");
        }

        public void OnOrderStatusChanged(Order order, string oldStatus, string newStatus)
        {
            Log($"STATUS_CHANGED | OrderID: {order.OrderID} | {oldStatus} → {newStatus}");
        }

        public void OnOrderCancelled(Order order)
        {
            Log($"ORDER_CANCELLED | OrderID: {order.OrderID} | Reason: User request");
        }

        public void OnOrderCompleted(Order order)
        {
            Log($"ORDER_COMPLETED | OrderID: {order.OrderID} | Revenue: ${order.TotalAmount - order.Discount:F2}");
        }

        private void Log(string message)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            System.Diagnostics.Debug.WriteLine($"[Log] {logMessage}");
            
            // In real implementation, write to file:
            // File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
        }
    }
}
