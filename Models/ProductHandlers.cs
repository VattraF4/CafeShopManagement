using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOADCafeShopManagement.Models
{

    class ProductHandlers:DbConnection
    {
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public int SupplierID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string Description { get; set; }
        public List<ProductHandlers> ListData()
        {
            List<ProductHandlers> productsList = new List<ProductHandlers>(); //by default access modifier is private
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT id, name, categories_id, supplier_id, price, discount FROM Products";
                using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProductHandlers product = new ProductHandlers
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                CategoryID = reader.GetInt32(2),
                                SupplierID = reader.GetInt32(3),
                                Price = reader.GetDecimal(4),
                                Discount = reader.GetDecimal(5)
                            };
                            productsList.Add(product);
                        }
                    }
                }
            }
            return productsList;
        }
        public bool  AddProduct(string name, int category,int supplier,decimal price , decimal discount)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                try
                {

                    string query = @"INSERT INTO Products 
                    (name, categories_id, supplier_id, price, discount) 
                    VALUES 
                    (@name, @categories_id, @supplier_id, @price, @discount)";

                    using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@categories_id", category);
                        command.Parameters.AddWithValue("@supplier_id", supplier);
                        command.Parameters.AddWithValue("@price", price);
                        command.Parameters.AddWithValue("@discount", discount);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error adding product: " + ex.Message);
                }
            }
            return
        }
    }
}
