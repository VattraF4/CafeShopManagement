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
        public string CategoryName { get; set; }
        public string SupplierName { get; set; }
        public List<ProductHandlers> ListData()
        {
            List<ProductHandlers> productsList = new List<ProductHandlers>(); //by default access modifier is private
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = @"SELECT
                                    p.id,
                                    p.name,
                                    p.price,
                                    p.discount,
                                    p.categories_id,
                                    p.supplier_id,
                                    c.name AS categoryName,
                                    s.name AS supplierName
                                FROM products p
                                JOIN categories c ON p.categories_id = c.id
                                JOIN suppliers s ON p.supplier_id = s.id;";

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
                                Price = reader.GetDecimal(2),
                                Discount = reader.GetDecimal(3),
                                CategoryID = reader.GetInt32(4),
                                SupplierID = reader.GetInt32(5),
                                CategoryName = reader.GetString(6),
                                SupplierName = reader.GetString(7)
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
                        int rowAffected = command.ExecuteNonQuery();  // this method is execute insert, update, delete and return number of rows affected
                        if (rowAffected <= 0)
                        {
                            throw new Exception("No rows were inserted.");
                        }
                        else
                        {
                            return true;
                        }

                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error adding product: " + ex.Message);
                }
            }
        }
    }
}
