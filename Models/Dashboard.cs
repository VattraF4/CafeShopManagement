//using OOADCafeShopManagement.Db;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OOADCafeShopManagement.Models
{
    public struct RevenueByDate
    {
        public string Date { get; set; }
        public decimal TotalAmount { get; set; }
    }
    // Add this class for better DataGridView support
    public class UnderstockProduct
    {
        public string ProductName { get; set; }
        public int Stock { get; set; }
        public decimal UnitPrice { get; set; }
        //public string Package { get; set; }
        public string Status => Stock <= 20 ? "Low Stock" : "In Stock";
    }
    public class Dashboard : DbConnection
    {
        //Fields & Properties
        private DateTime startDate;
        private DateTime endDate;
        private int numberDays;

        public int NumberSuppliers { get; set; }
        public int NumberProducts { get; set; }
        public int NumberEmployees { get; set; }
        public int NumberCustomers { get; set; }

        public List<KeyValuePair<string, int>> TopProductsList { get; private set; }
        public List<UnderstockProduct> UnderstockList { get; private set; } // Changed to custom class
        public List<RevenueByDate> GrossRevenueList { get; private set; }
        public int NumberOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }

        //Constructor
        public Dashboard()
        {

        }
        //Private Methods
        private void GetNumberItems()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                // Implement logic to retrieve number of suppliers, products, employees, and customers
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    // Example for Number of users
                    command.CommandText = "SELECT COUNT(*) FROM Supplier";
                    NumberSuppliers = (int)command.ExecuteScalar();

                    // Similarly implement for Products, Employees, and Customers

                    //Get Total of Products
                    command.CommandText = "SELECT COUNT(*) FROM product";
                    NumberProducts = (int)command.ExecuteScalar();

                    //Get total number of Employees
                    command.CommandText = "SELECT COUNT(*) FROM users";
                    NumberEmployees = (int)command.ExecuteScalar();
                    //Get total number of Orders
                    command.CommandText = @"SELECT count(id) from [Order] WHERE OrderDate between @startDate AND @endDate";
                    //command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.Add("@startDate", System.Data.SqlDbType.DateTime).Value = startDate;
                    command.Parameters.Add("@endDate", System.Data.SqlDbType.DateTime).Value = endDate;
                    NumberOrders = (int)command.ExecuteScalar();
                }
            }
        }
        private void GetOrderAnalysis()
        {
            GrossRevenueList = new List<RevenueByDate>();
            TotalProfit = 0;
            TotalRevenue = 0;

            using (var connection = GetConnection())
            {
                connection.Open();
                // Implement logic to retrieve number of suppliers, products, employees, and customers
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT OrderDate, sum(TotalAmount) From [Order]
                                            WHERE OrderDate between @fromDate and @toDate group by OrderDate";
                    command.Parameters.Add("@fromDate", System.Data.SqlDbType.DateTime).Value = startDate;

                    command.Parameters.Add("@toDate", System.Data.SqlDbType.DateTime).Value = endDate;
                    var reader = command.ExecuteReader();
                    var resultTable = new List<KeyValuePair<DateTime, decimal>>();
                    while (reader.Read())
                    {
                        resultTable.Add(
                            new KeyValuePair<DateTime, decimal>((DateTime)reader[0], (decimal)reader[1])
                            );
                        TotalRevenue += (decimal)reader[1];

                    }
                    TotalProfit = TotalRevenue * 0.2m; //20%
                    reader.Close();

                    //Group by Days
                    if (numberDays <= 30)
                    {
                        foreach (var item in resultTable)
                        {
                            GrossRevenueList.Add(new RevenueByDate()
                            {
                                Date = item.Key.ToString("ddd MMM"),
                                TotalAmount = item.Value
                            });
                        }
                    }
                    //Group by Week
                    else if (numberDays <= 92)
                    {
                        GrossRevenueList = (from orderList in resultTable
                                            group orderList by CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                                                    orderList.Key, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                                            into order
                                            select new RevenueByDate
                                            {
                                                Date = "Week" + order.Key.ToString(),
                                                TotalAmount = order.Sum(amount => amount.Value)
                                            }).ToList();
                    }
                    else if (numberDays <= (365 * 2))
                    {
                        bool isYear = numberDays <= 365 ? true : false;
                        GrossRevenueList = (from orderList in resultTable
                                            group orderList by orderList.Key.ToString("MMM yyyy")
                                            into order
                                            select new RevenueByDate
                                            {
                                                Date = isYear ? order.Key.Substring(0, order.Key.IndexOf(" ")) : order.Key,
                                                TotalAmount = order.Sum(amount => amount.Value)
                                            }).ToList();
                    }
                    //Group by Year
                    else
                    {
                        GrossRevenueList = (from orderList in resultTable
                                            group orderList by orderList.Key.ToString("yyyy")
                                            into order
                                            select new RevenueByDate
                                            {
                                                Date = order.Key,
                                                TotalAmount = order.Sum(amount => amount.Value)
                                            }).ToList();
                    }
                }

            }
        }

        private void GetProductAnalysis()
        {
            TopProductsList = new List<KeyValuePair<string, int>>();
            UnderstockList = new List<UnderstockProduct>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    SqlDataReader reader;
                    command.Connection = connection;

                    //Top 5 Products
                    command.CommandText = @"SELECT  TOP 5 p.ProductName, sum(OrderItem.Quantity) as Q FROM  OrderItem
                                            INNER JOIN  Product P  on P.Id = OrderItem.ProductId
                                            INNER JOIN  [Order] O on O.Id = OrderItem.OrderId
                                            WHERE  OrderDate between @fromDate and @toDate
                                            GROUP BY p.ProductName ORDER BY  Q DESC ;";

                    command.Parameters.Add("@fromDate", System.Data.SqlDbType.DateTime).Value = startDate;
                    command.Parameters.Add("@toDate", System.Data.SqlDbType.DateTime).Value = endDate;
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        TopProductsList.Add(new KeyValuePair<string, int>(reader[0].ToString(), (int)reader[1]));
                    }
                    reader.Close();
                    //Understock Products
                    //command.Connection = connection;
                    command.CommandText = @"SELECT ProductName, Stock, UnitPrice FROM Product
                                            WHERE Stock <= 100
                                            ORDER BY Stock DESC;";
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        UnderstockList.Add(new UnderstockProduct
                        {
                            ProductName = reader["ProductName"].ToString(),
                            Stock = Convert.ToInt32(reader["Stock"]),
                            UnitPrice = Convert.ToDecimal(reader["UnitPrice"])
                            //,Package = reader["Package"].ToString()
                        });
                    }
                    reader.Close();
                }
            }

        }

        //Public Methods
        public bool LoadData(DateTime startDate, DateTime endDate)
        {
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, endDate.Hour, endDate.Minute, endDate.Second, 59);
            if (startDate != this.startDate || endDate != this.endDate)
            {
                this.startDate = startDate;
                this.endDate = endDate;
                numberDays = (endDate - startDate).Days;  // This statement is use accessor set within the class by properties Date

                GetNumberItems();
                GetOrderAnalysis();
                GetProductAnalysis();
                Console.WriteLine("Refreshed dated {0} - {1}", startDate.ToString(), endDate.ToString());
                return true;
            }
            else
            {
                Console.WriteLine("Date range not refreshed, same query: {0} -{1}", startDate.ToString(), endDate.ToString());
                return false;
            }
        }
    };
}
