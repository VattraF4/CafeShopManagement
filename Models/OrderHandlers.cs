using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOADCafeShopManagement.Models
{
    internal class OrderHandlers : DbConnection
    {
        //Properties
        public int OrderID { get; set; }
        public string Note { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public int UserID { set; get; }
        public DateTime OrderDate { get; set; }
        public List<OrderDetailHandler> orderDetailHandlers { get; set; } = new List<OrderDetailHandler>();

        public List<OrderHandlers> ListData()
        {
            List<OrderHandlers> orderList = new List<OrderHandlers>();
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                                                // 0, 1,    2       3,          4,           5 
                    command.CommandText = "SELECT id,note, status,created_at, discount, total_amount FROM Orders";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var order = new OrderHandlers
                            {
                                OrderID = reader.GetInt32(0),
                                Note = reader.GetString(1),
                                Status = reader.GetString(2),
                                OrderDate = reader.GetDateTime(3),
                                Discount = reader.GetDecimal(4),
                                TotalAmount = reader.GetDecimal(5)
                            };
                            orderList.Add(order);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return orderList;
        }

    }

}

