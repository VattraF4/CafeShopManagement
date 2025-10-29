//using OOADCafeShopManagement.Db;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOADCafeShopManagement.Models
{
    public struct RevenueByDate
    {
        public string Date { get; set; }
        public decimal TotalAmount { get; set; }
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
        public List<KeyValuePair<string, int>> UnderstockList { get; private set; }
        public List<RevenueByDate> GrossRevenueList { get; private set; }
        public int NumberOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }

        //contructor
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
                    command.CommandText = "SELECT COUNT(*) FROM users";
                    NumberSuppliers = (int)command.ExecuteScalar();

                    // Similarly implement for Products, Employees, and Customers

                    //Get Total of Products
                    command.CommandText = "SELECT COUNT(*) FROM products";

                    //Get total number of Orders
                    command.CommandText = @"SELECT count(id) from [order]" +
                        "WHERE order_date between @startDate AND @endDate";
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
                    command.CommandText = @"SELECT order_date, sum(TotalAmount) From orders
                        WHERE order_date between @fromDate and @toDate group by order_date";
                    command.Parameters.Add("@fromDate", System.Data.SqlDbType.DateTime).Value = startDate;

                    command.Parameters.Add("@toDate", System.Data.SqlDbType.DateTime).Value = endDate;
                    var reader = command.ExecuteReader();
                    var resultTable = new List<KeyValuePair<DateTime, decimal>>();
                    while (reader.Read())
                    {
                        resultTable.Add(
                            new KeyValuePair<DateTime, decimal>((DateTime)reader[0], (decimal)reader[1])
                            );
                        TotalRevenue += (decimal)reader[0];

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
                                                Date = isYear ? order.Key.Substring(0,order.Key.IndexOf(" ")) : order.Key,
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
    };
}
