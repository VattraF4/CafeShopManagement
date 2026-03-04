using System;
using System.Collections.Generic;
using System.Linq;
using OOADCafeShopManagement.Interface;
using OOADCafeShopManagement.Models;
using OOADCafeShopManagement.Factory;

namespace OOADCafeShopManagement.Services
{
    /// <summary>
    /// Order Service Layer - Business logic for order management
    /// Uses Repository Pattern for data access
    /// Uses Factory Pattern for object creation
    /// </summary>
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderFactory _orderFactory;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _orderFactory = new OrderFactory();
        }

        public OrderService(IOrderRepository orderRepository, IOrderFactory orderFactory)
        {
            _orderRepository = orderRepository;
            _orderFactory = orderFactory;
        }

        public List<Order> GetAllOrders()
        {
            return _orderRepository.GetAllOrders();
        }

        public Order GetOrderById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Order ID must be greater than zero.", nameof(id));
            
            return _orderRepository.GetOrderById(id);
        }

        public List<Order> GetOrdersByUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be greater than zero.", nameof(userId));
            
            return _orderRepository.GetOrdersByUserId(userId);
        }

        public List<Order> GetOrdersByStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status is required.", nameof(status));
            
            return _orderRepository.GetOrdersByStatus(status);
        }

        public List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start date cannot be after end date.");
            
            return _orderRepository.GetOrdersByDateRange(startDate, endDate);
        }

        public bool AddOrder(int userId, decimal totalAmount, decimal discount = 0, string note = null, string status = "Pending")
        {
            // Use OrderFactory to create order object with validation
            var order = _orderFactory.CreatePendingOrder(userId, totalAmount, discount, note);

            if (!string.IsNullOrWhiteSpace(status))
            {
                order.Status = status;
            }

            return _orderRepository.AddOrder(order);
        }

        // New methods using OrderFactory
        public bool CreateDineInOrder(int userId, int tableNumber, decimal totalAmount = 0, decimal discount = 0)
        {
            var order = _orderFactory.CreateDineInOrder(userId, tableNumber);
            order.TotalAmount = totalAmount;
            order.Discount = discount;
            return _orderRepository.AddOrder(order);
        }

        public bool CreateTakeawayOrder(int userId, string customerName, decimal totalAmount = 0, decimal discount = 0)
        {
            var order = _orderFactory.CreateTakeawayOrder(userId, customerName);
            order.TotalAmount = totalAmount;
            order.Discount = discount;
            return _orderRepository.AddOrder(order);
        }

        public bool CreateDeliveryOrder(int userId, string deliveryAddress, string customerName, 
            string phone, decimal totalAmount = 0, decimal discount = 0)
        {
            var order = _orderFactory.CreateDeliveryOrder(userId, deliveryAddress, customerName, phone);
            order.TotalAmount = totalAmount;
            order.Discount = discount;
            return _orderRepository.AddOrder(order);
        }

        public bool UpdateOrder(int orderId, string status, decimal? totalAmount = null, decimal? discount = null, string note = null)
        {
            if (orderId <= 0)
                throw new ArgumentException("Invalid order ID.", nameof(orderId));
            
            // Get existing order
            var existingOrder = _orderRepository.GetOrderById(orderId);
            if (existingOrder == null)
            {
                throw new Exception("Order not found.");
            }

            // Update properties
            existingOrder.Status = status ?? existingOrder.Status;
            
            if (totalAmount.HasValue)
            {
                if (totalAmount.Value < 0)
                    throw new ArgumentException("Total amount cannot be negative.");
                existingOrder.TotalAmount = totalAmount.Value;
            }
            
            if (discount.HasValue)
            {
                if (discount.Value < 0)
                    throw new ArgumentException("Discount cannot be negative.");
                if (discount.Value > existingOrder.TotalAmount)
                    throw new ArgumentException("Discount cannot exceed total amount.");
                existingOrder.Discount = discount.Value;
            }
            
            if (note != null)
            {
                existingOrder.Note = note;
            }

            return _orderRepository.UpdateOrder(existingOrder);
        }

        public bool UpdateOrderStatus(int orderId, string newStatus)
        {
            if (orderId <= 0)
                throw new ArgumentException("Invalid order ID.", nameof(orderId));
            
            if (string.IsNullOrWhiteSpace(newStatus))
                throw new ArgumentException("Status is required.", nameof(newStatus));

            var order = _orderRepository.GetOrderById(orderId);
            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            order.Status = newStatus;
            return _orderRepository.UpdateOrder(order);
        }

        public bool CompleteOrder(int orderId)
        {
            return UpdateOrderStatus(orderId, "Completed");
        }

        public bool CancelOrder(int orderId)
        {
            return UpdateOrderStatus(orderId, "Cancelled");
        }

        public bool ProcessOrder(int orderId)
        {
            return UpdateOrderStatus(orderId, "Processing");
        }

        public bool DeleteOrder(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid order ID.", nameof(id));

            var order = _orderRepository.GetOrderById(id);
            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            return _orderRepository.DeleteOrder(id);
        }

        // Business analytics methods
        public List<Order> GetPendingOrders()
        {
            return _orderRepository.GetOrdersByStatus("Pending");
        }

        public List<Order> GetCompletedOrders()
        {
            return _orderRepository.GetOrdersByStatus("Completed");
        }

        public List<Order> GetCancelledOrders()
        {
            return _orderRepository.GetOrdersByStatus("Cancelled");
        }

        public List<Order> GetTodayOrders()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            return _orderRepository.GetOrdersByDateRange(today, tomorrow);
        }

        public List<Order> GetThisWeekOrders()
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);
            return _orderRepository.GetOrdersByDateRange(startOfWeek, endOfWeek);
        }

        public List<Order> GetThisMonthOrders()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);
            return _orderRepository.GetOrdersByDateRange(startOfMonth, endOfMonth);
        }

        public decimal GetTotalRevenue()
        {
            var allOrders = GetCompletedOrders();
            return allOrders.Sum(o => o.TotalAmount - o.Discount);
        }

        public decimal GetTotalRevenueByDateRange(DateTime startDate, DateTime endDate)
        {
            var orders = _orderRepository.GetOrdersByDateRange(startDate, endDate)
                .Where(o => o.Status == "Completed").ToList();
            return orders.Sum(o => o.TotalAmount - o.Discount);
        }

        public decimal GetAverageOrderValue()
        {
            var completedOrders = GetCompletedOrders();
            if (completedOrders.Count == 0) return 0;
            return completedOrders.Average(o => o.TotalAmount - o.Discount);
        }

        public int GetTotalOrderCount()
        {
            return GetAllOrders().Count;
        }

        public int GetCompletedOrderCount()
        {
            return GetCompletedOrders().Count;
        }

        public Order GetLargestOrder()
        {
            var completedOrders = GetCompletedOrders();
            return completedOrders.OrderByDescending(o => o.TotalAmount - o.Discount).FirstOrDefault();
        }
    }
}
