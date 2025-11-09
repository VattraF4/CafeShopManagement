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
    public class ItemsSoldByDate
    {
        public string Date { get; set; }
        public int Quantity { get; set; }
    }
    // Add this class for better DataGridView support
    public class UnderstockProduct
    {
        public int ID { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public string Status { get; set; }
        //public string Package { get; set; }
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
        public int TotalItemsSold { get; private set; }

        public List<KeyValuePair<string, int>> TopProductsList { get; private set; }
        public List<UnderstockProduct> UnderstockList { get; private set; } // Changed to custom class
        public List<RevenueByDate> GrossRevenueList { get; private set; }
        public List<ItemsSoldByDate> ItemsSoldList { get; private set; }
        public int NumberOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }
        //public int TotalItemsSold { get; set; }

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
                    command.CommandText = "SELECT COUNT(*) FROM suppliers";
                    NumberSuppliers = (int)command.ExecuteScalar();

                    // Similarly implement for Products, Employees, and Customers

                    //Get Total of Products
                    command.CommandText = "SELECT COUNT(*) FROM products";
                    NumberProducts = (int)command.ExecuteScalar();

                    //Get total number of Employees
                    command.CommandText = "SELECT COUNT(*) FROM users";
                    NumberEmployees = (int)command.ExecuteScalar();
                    //Get total number of Orders
                    command.CommandText = @"SELECT count(id) from [orders] WHERE created_at between @startDate AND @endDate";
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
            ItemsSoldList = new List<ItemsSoldByDate>(); // New list for items sold
            TotalProfit = 0;
            TotalRevenue = 0;
            TotalItemsSold = 0;

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    // Query for revenue from Orders table
                    command.CommandText = @"SELECT created_at, SUM(total_amount) 
                                    FROM [Orders]
                                    WHERE created_at BETWEEN @fromDate AND @toDate 
                                    GROUP BY created_at;";
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
                    reader.Close();

                    // Query for items sold from order_details table
                    command.CommandText = @"SELECT created_at, SUM(quantity) as total_items_sold
                                    FROM order_details
                                    WHERE created_at BETWEEN @fromDate AND @toDate 
                                    GROUP BY created_at;";

                    reader = command.ExecuteReader();
                    var itemsSoldTable = new List<KeyValuePair<DateTime, int>>();
                    while (reader.Read())
                    {
                        itemsSoldTable.Add(
                            new KeyValuePair<DateTime, int>((DateTime)reader[0], (int)reader[1])
                        );
                        TotalItemsSold += (int)reader[1];
                    }
                    reader.Close();

                    TotalProfit = TotalRevenue * 0.2m; // 20%

                    // Group by Days (for both revenue and items sold)
                    if (numberDays <= 30)
                    {
                        // Revenue grouping
                        foreach (var item in resultTable)
                        {
                            GrossRevenueList.Add(new RevenueByDate()
                            {
                                Date = item.Key.ToString("ddd MMM"),
                                TotalAmount = item.Value
                            });
                        }

                        // Items sold grouping
                        foreach (var item in itemsSoldTable)
                        {
                            ItemsSoldList.Add(new ItemsSoldByDate()
                            {
                                Date = item.Key.ToString("ddd MMM"),
                                Quantity = item.Value
                            });
                        }
                    }
                    // Group by Week
                    else if (numberDays <= 92)
                    {
                        // Revenue by week
                        GrossRevenueList = (from orderList in resultTable
                                            group orderList by CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                                                    orderList.Key, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                                            into order
                                            select new RevenueByDate
                                            {
                                                Date = "Week " + order.Key.ToString(),
                                                TotalAmount = order.Sum(amount => amount.Value)
                                            }).ToList();

                        // Items sold by week
                        ItemsSoldList = (from itemList in itemsSoldTable
                                         group itemList by CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                                                 itemList.Key, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                                         into week
                                         select new ItemsSoldByDate
                                         {
                                             Date = "Week " + week.Key.ToString(),
                                             Quantity = week.Sum(item => item.Value)
                                         }).ToList();
                    }
                    // Group by Month
                    else if (numberDays <= (365 * 2))
                    {
                        bool isYear = numberDays <= 365;

                        // Revenue by month
                        GrossRevenueList = (from orderList in resultTable
                                            group orderList by orderList.Key.ToString("MMM yyyy")
                                            into order
                                            select new RevenueByDate
                                            {
                                                Date = isYear ? order.Key.Substring(0, order.Key.IndexOf(" ")) : order.Key,
                                                TotalAmount = order.Sum(amount => amount.Value)
                                            }).ToList();

                        // Items sold by month
                        ItemsSoldList = (from itemList in itemsSoldTable
                                         group itemList by itemList.Key.ToString("MMM yyyy")
                                         into month
                                         select new ItemsSoldByDate
                                         {
                                             Date = isYear ? month.Key.Substring(0, month.Key.IndexOf(" ")) : month.Key,
                                             Quantity = month.Sum(item => item.Value)
                                         }).ToList();
                    }
                    // Group by Year
                    else
                    {
                        // Revenue by year
                        GrossRevenueList = (from orderList in resultTable
                                            group orderList by orderList.Key.ToString("yyyy")
                                            into order
                                            select new RevenueByDate
                                            {
                                                Date = order.Key,
                                                TotalAmount = order.Sum(amount => amount.Value)
                                            }).ToList();

                        // Items sold by year
                        ItemsSoldList = (from itemList in itemsSoldTable
                                         group itemList by itemList.Key.ToString("yyyy")
                                         into year
                                         select new ItemsSoldByDate
                                         {
                                             Date = year.Key,
                                             Quantity = year.Sum(item => item.Value)
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
                    command.CommandText = @"SELECT TOP 5
                                                p.name as ProductName,
                                                SUM(od.quantity) as TotalQuantityOrdered
                                            FROM order_details od
                                            INNER JOIN products p ON p.id = od.product_id  -- Only need this join!
                                            WHERE od.created_at BETWEEN @fromDate AND @toDate
                                            GROUP BY p.name
                                            ORDER BY TotalQuantityOrdered DESC;";

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
                    command.CommandText = @"SELECT 
                                           p.id as Id,
                                           p.name   as ProductName,
                                           p.price  as Price,
                                           p.status as Status
                                    FROM products p
                                    ORDER BY Id;";
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        UnderstockList.Add(new UnderstockProduct
                        {
                            ID = Convert.ToInt32(reader["Id"]),
                            ProductName = reader["ProductName"].ToString(),
                            UnitPrice = Convert.ToDecimal(reader["Price"]),
                            Status = reader["Status"].ToString()
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
