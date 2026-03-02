using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOADCafeShopManagement
{
    public class DbConnection
    {
        private static DbConnection _instance;
        private static readonly object _lock = new object();
        private readonly string connectionString;

        private DbConnection()
        {
            //Below is Method 1 for connectionString
            //connectionString = "Server=(local);DataBase=cafe; Integrated Security = true";

            //Below is Method 2 for connectionString
            connectionString = @"Data Source=.\SQLEXPRESS;
                Initial Catalog=cafe;
                Integrated Security=True;
                TrustServerCertificate=True";

            //SqlConnection connection = new SqlConnection
            //    (@"Data Source=.\SQLEXPRESS;
            //    Initial Catalog=cafe;
            //    Integrated Security=True;
            //    TrustServerCertificate=True");
        }

        public static DbConnection Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DbConnection();
                        }
                    }
                }
                return _instance;
            }
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        } 
    }
}
