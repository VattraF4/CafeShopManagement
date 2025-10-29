using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOADCafeShopManagement
{
    public abstract class DbConnection
    {
        private readonly string connectionString;

        public DbConnection()
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
        protected SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        } 
    }
}
