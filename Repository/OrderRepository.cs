using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using OOADCafeShopManagement.Interface;
using OOADCafeShopManagement.Models;

namespace OOADCafeShopManagement.Repository
{
    public class OrderRepository : IOrderRepository
    {
        public List<Order> GetAllOrders()
        {
            List<Order> orderList = new List<Order>();
            try
            {
                using (var connection = DbConnection.Instance.GetConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT id, note, status, created_at, discount, total_amount, user_id FROM Orders";
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var order = new Order
                            {
                                OrderID = reader.GetInt32(0),
                                Note = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                Status = reader.GetString(2),
                                OrderDate = reader.GetDateTime(3),
                                Discount = reader.GetDecimal(4),
                                TotalAmount = reader.GetDecimal(5),
                                UserID = reader.GetInt32(6)
                            };
                            orderList.Add(order);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving orders: " + ex.Message);
                throw;
            }
            return orderList;
        }

        public Order GetOrderById(int id)
        {
            try
            {
                using (var connection = DbConnection.Instance.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT id, note, status, created_at, discount, total_amount, user_id FROM Orders WHERE id = @id";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Order
                                {
                                    OrderID = reader.GetInt32(0),
                                    Note = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                    Status = reader.GetString(2),
                                    OrderDate = reader.GetDateTime(3),
                                    Discount = reader.GetDecimal(4),
                                    TotalAmount = reader.GetDecimal(5),
                                    UserID = reader.GetInt32(6)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting order: " + ex.Message);
                throw;
            }
            return null;
        }

        public bool AddOrder(Order order)
        {
            try
            {
                using (var connection = DbConnection.Instance.GetConnection())
                {
                    connection.Open();
                    string query = @"INSERT INTO Orders (status, total_amount, discount, user_id, note) 
                                   VALUES (@Status, @TotalAmount, @Discount, @UserId, @Note)";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Status", order.Status);
                        command.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
                        command.Parameters.AddWithValue("@Discount", order.Discount);
                        command.Parameters.AddWithValue("@UserId", order.UserID);
                        command.Parameters.AddWithValue("@Note", order.Note ?? (object)DBNull.Value);
                        
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding order: " + ex.Message);
                throw;
            }
        }

        public bool UpdateOrder(Order order)
        {
            try
            {
                using (var connection = DbConnection.Instance.GetConnection())
                {
                    connection.Open();
                    string query = @"UPDATE Orders SET 
                                   status = @Status, 
                                   total_amount = @TotalAmount, 
                                   discount = @Discount, 
                                   user_id = @UserId, 
                                   note = @Note 
                                   WHERE id = @Id";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", order.OrderID);
                        command.Parameters.AddWithValue("@Status", order.Status);
                        command.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
                        command.Parameters.AddWithValue("@Discount", order.Discount);
                        command.Parameters.AddWithValue("@UserId", order.UserID);
                        command.Parameters.AddWithValue("@Note", order.Note ?? (object)DBNull.Value);
                        
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating order: " + ex.Message);
                throw;
            }
        }

        public bool DeleteOrder(int id)
        {
            try
            {
                using (var connection = DbConnection.Instance.GetConnection())
                {
                    connection.Open();
                    string query = "DELETE FROM Orders WHERE id = @id";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting order: " + ex.Message);
                throw;
            }
        }

        public List<Order> GetOrdersByUserId(int userId)
        {
            List<Order> orderList = new List<Order>();
            try
            {
                using (var connection = DbConnection.Instance.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT id, note, status, created_at, discount, total_amount, user_id FROM Orders WHERE user_id = @userId";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var order = new Order
                                {
                                    OrderID = reader.GetInt32(0),
                                    Note = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                    Status = reader.GetString(2),
                                    OrderDate = reader.GetDateTime(3),
                                    Discount = reader.GetDecimal(4),
                                    TotalAmount = reader.GetDecimal(5),
                                    UserID = reader.GetInt32(6)
                                };
                                orderList.Add(order);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting orders by user: " + ex.Message);
                throw;
            }
            return orderList;
        }

        public List<Order> GetOrdersByStatus(string status)
        {
            List<Order> orderList = new List<Order>();
            try
            {
                using (var connection = DbConnection.Instance.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT id, note, status, created_at, discount, total_amount, user_id FROM Orders WHERE status = @status";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@status", status);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var order = new Order
                                {
                                    OrderID = reader.GetInt32(0),
                                    Note = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                    Status = reader.GetString(2),
                                    OrderDate = reader.GetDateTime(3),
                                    Discount = reader.GetDecimal(4),
                                    TotalAmount = reader.GetDecimal(5),
                                    UserID = reader.GetInt32(6)
                                };
                                orderList.Add(order);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting orders by status: " + ex.Message);
                throw;
            }
            return orderList;
        }

        public List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            List<Order> orderList = new List<Order>();
            try
            {
                using (var connection = DbConnection.Instance.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT id, note, status, created_at, discount, total_amount, user_id FROM Orders WHERE created_at BETWEEN @startDate AND @endDate";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@startDate", startDate);
                        command.Parameters.AddWithValue("@endDate", endDate);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var order = new Order
                                {
                                    OrderID = reader.GetInt32(0),
                                    Note = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                    Status = reader.GetString(2),
                                    OrderDate = reader.GetDateTime(3),
                                    Discount = reader.GetDecimal(4),
                                    TotalAmount = reader.GetDecimal(5),
                                    UserID = reader.GetInt32(6)
                                };
                                orderList.Add(order);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting orders by date range: " + ex.Message);
                throw;
            }
            return orderList;
        }
    }
}
