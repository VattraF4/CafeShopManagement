using System;
using OOADCafeShopManagement.Models;

namespace OOADCafeShopManagement.Factory
{
    /// <summary>
    /// Factory Pattern for creating Order objects
    /// Handles different order types and initialization
    /// </summary>
    public interface IOrderFactory
    {
        Order CreateOrder(int userId, string orderType = "Standard");
        Order CreatePendingOrder(int userId, decimal totalAmount, decimal discount = 0, string note = null);
        Order CreateCompletedOrder(int userId, decimal totalAmount, decimal discount = 0, string note = null);
        Order CreateDineInOrder(int userId, int tableNumber);
        Order CreateTakeawayOrder(int userId, string customerName = null);
        Order CreateDeliveryOrder(int userId, string deliveryAddress, string customerName = null, string phone = null);
    }

    public class OrderFactory : IOrderFactory
    {
        /// <summary>
        /// Creates a standard order with the specified type
        /// </summary>
        public Order CreateOrder(int userId, string orderType = "Standard")
        {
            if (userId <= 0)
                throw new ArgumentException("Valid user ID is required.", nameof(userId));

            return new Order
            {
                UserID = userId,
                Status = "Pending",
                OrderDate = DateTime.Now,
                TotalAmount = 0,
                Discount = 0,
                Note = $"Order Type: {orderType}"
            };
        }

        /// <summary>
        /// Creates a pending order ready for items to be added
        /// </summary>
        public Order CreatePendingOrder(int userId, decimal totalAmount, decimal discount = 0, string note = null)
        {
            if (userId <= 0)
                throw new ArgumentException("Valid user ID is required.", nameof(userId));

            if (totalAmount < 0)
                throw new ArgumentException("Total amount cannot be negative.", nameof(totalAmount));

            if (discount < 0)
                throw new ArgumentException("Discount cannot be negative.", nameof(discount));

            if (discount > totalAmount)
                throw new ArgumentException("Discount cannot exceed total amount.");

            return new Order
            {
                UserID = userId,
                Status = "Pending",
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                Discount = discount,
                Note = note
            };
        }

        /// <summary>
        /// Creates a completed order (for direct completion scenarios)
        /// </summary>
        public Order CreateCompletedOrder(int userId, decimal totalAmount, decimal discount = 0, string note = null)
        {
            if (userId <= 0)
                throw new ArgumentException("Valid user ID is required.", nameof(userId));

            if (totalAmount < 0)
                throw new ArgumentException("Total amount cannot be negative.", nameof(totalAmount));

            return new Order
            {
                UserID = userId,
                Status = "Completed",
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                Discount = discount,
                Note = note
            };
        }

        /// <summary>
        /// Creates a dine-in order with table information
        /// </summary>
        public Order CreateDineInOrder(int userId, int tableNumber)
        {
            if (userId <= 0)
                throw new ArgumentException("Valid user ID is required.", nameof(userId));

            if (tableNumber <= 0)
                throw new ArgumentException("Valid table number is required.", nameof(tableNumber));

            return new Order
            {
                UserID = userId,
                Status = "Pending",
                OrderDate = DateTime.Now,
                TotalAmount = 0,
                Discount = 0,
                Note = $"Dine-in | Table: {tableNumber}"
            };
        }

        /// <summary>
        /// Creates a takeaway order
        /// </summary>
        public Order CreateTakeawayOrder(int userId, string customerName = null)
        {
            if (userId <= 0)
                throw new ArgumentException("Valid user ID is required.", nameof(userId));

            string note = "Takeaway";
            if (!string.IsNullOrWhiteSpace(customerName))
            {
                note += $" | Customer: {customerName}";
            }

            return new Order
            {
                UserID = userId,
                Status = "Pending",
                OrderDate = DateTime.Now,
                TotalAmount = 0,
                Discount = 0,
                Note = note
            };
        }

        /// <summary>
        /// Creates a delivery order with customer details
        /// </summary>
        public Order CreateDeliveryOrder(int userId, string deliveryAddress, string customerName = null, string phone = null)
        {
            if (userId <= 0)
                throw new ArgumentException("Valid user ID is required.", nameof(userId));

            if (string.IsNullOrWhiteSpace(deliveryAddress))
                throw new ArgumentException("Delivery address is required.", nameof(deliveryAddress));

            string note = $"Delivery | Address: {deliveryAddress}";
            
            if (!string.IsNullOrWhiteSpace(customerName))
            {
                note += $" | Customer: {customerName}";
            }
            
            if (!string.IsNullOrWhiteSpace(phone))
            {
                note += $" | Phone: {phone}";
            }

            return new Order
            {
                UserID = userId,
                Status = "Pending",
                OrderDate = DateTime.Now,
                TotalAmount = 0,
                Discount = 0,
                Note = note
            };
        }

        /// <summary>
        /// Creates an order from database data
        /// </summary>
        public Order CreateFromData(int orderId, int userId, string status, 
            DateTime orderDate, decimal totalAmount, decimal discount, string note)
        {
            return new Order
            {
                OrderID = orderId,
                UserID = userId,
                Status = status,
                OrderDate = orderDate,
                TotalAmount = totalAmount,
                Discount = discount,
                Note = note
            };
        }
    }
}
