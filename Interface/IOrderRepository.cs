using System;
using System.Collections.Generic;
using OOADCafeShopManagement.Models;

namespace OOADCafeShopManagement.Interface
{
    public interface IOrderRepository
    {
        List<Order> GetAllOrders();
        Order GetOrderById(int id);
        bool AddOrder(Order order);
        bool UpdateOrder(Order order);
        bool DeleteOrder(int id);
        List<Order> GetOrdersByUserId(int userId);
        List<Order> GetOrdersByStatus(string status);
        List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate);
    }
}
