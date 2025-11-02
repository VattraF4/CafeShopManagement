using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOADCafeShopManagement.Models
{
    class SupplierHandler:DbConnection
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ContactName { get;set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        
        public List<SupplierHandler> ListData()
        {
            List<SupplierHandler> suppliersList = new List<SupplierHandler>();
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT id, name, contact_name, phone, address, email FROM Suppliers Order By name";
                using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SupplierHandler supplier = new SupplierHandler
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                ContactName = reader.GetString(2),
                                PhoneNumber = reader.GetString(3),
                                Address = reader.GetString(4),
                                Email = reader.GetString(5)
                            };
                            suppliersList.Add(supplier);
                        }
                    }
                }
            }
            return suppliersList;
        }

    }
}
