using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOADCafeShopManagement.Models
{

    public class Products
    {


        //Properties
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public int SupplierID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string CategoryName { get; set; }
        public string SupplierName { get; set; }
        public string Status { get; set; }
        public List<Products> ListData()
        {
            List<Products> productsList = new List<Products>(); //by default access modifier is private
            using (var connection = DbConnection.Instance.GetConnection())
            {
                connection.Open();
                string query = @"SELECT
                                    p.id,
                                    p.name,
                                    p.price,
                                    p.discount,
                                    p.categories_id,
                                    p.supplier_id,
                                    p.status,
                                    c.name AS categoryName,
                                    s.name AS supplierName
                                FROM products p
                                JOIN categories c ON p.categories_id = c.id
                                JOIN suppliers s ON p.supplier_id = s.id order by p.id;";

                using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Products product = new Products
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                Discount = reader.GetDecimal(3),
                                CategoryID = reader.GetInt32(4),
                                SupplierID = reader.GetInt32(5),
                                CategoryName = reader.GetString(6),
                                SupplierName = reader.GetString(7),
                                Status = reader.GetString(8)
                            };
                            productsList.Add(product);
                        }
                    }
                }
            }
            return productsList;
        }
        public List<Products> ListActiveMenu()
        {
            List<Products> productsList = new List<Products>(); //by default access modifier is private
            using (var connection = DbConnection.Instance.GetConnection())
            {
                connection.Open();
                string query = @"SELECT
                                    p.id,
                                    p.name,
                                    p.price,
                                    p.discount,
                                    p.categories_id,
                                    p.supplier_id,
                                    p.status,
                                    c.name AS categoryName,
                                    s.name AS supplierName
                                FROM products p
                                JOIN categories c ON p.categories_id = c.id
                                JOIN suppliers s ON p.supplier_id = s.id  WHERE p.status = 'active' order by p.id;";

                using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Products product = new Products
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                Discount = reader.GetDecimal(3),
                                CategoryID = reader.GetInt32(4),
                                SupplierID = reader.GetInt32(5),
                                CategoryName = reader.GetString(6),
                                SupplierName = reader.GetString(7),
                                Status = reader.GetString(8)
                            };
                            productsList.Add(product);
                        }
                    }
                }
            }
            return productsList;
        }
        public bool AddProduct(string name, int category, int supplier, decimal price, decimal discount)
        {
            using (var connection = DbConnection.Instance.GetConnection())
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
        public bool UpdateProduct(int id, string name, int category, int supplier, decimal price, decimal discount, string status)
        {
            using (var connection = DbConnection.Instance.GetConnection())
            {
                connection.Open();
                try
                {
                    string query = @"UPDATE Products SET 
                    name = @name, 
                    categories_id = @categories_id, 
                    supplier_id = @supplier_id, 
                    price = @price, 
                    discount = @discount,
                    status = @status
                    WHERE id = @id";
                    using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@categories_id", category);
                        command.Parameters.AddWithValue("@supplier_id", supplier);
                        command.Parameters.AddWithValue("@price", price);
                        command.Parameters.AddWithValue("@discount", discount);
                        command.Parameters.AddWithValue("@status", status);
                        int rowAffected = command.ExecuteNonQuery();  // this method is execute insert, update, delete and return number of rows affected
                        if (rowAffected <= 0)
                        {
                            throw new Exception("No rows were updated.");
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error updating product: " + ex.Message);
                }
            }
        }
        public bool DeleteProduct(int id)
        {
            using (var connection = DbConnection.Instance.GetConnection())
            {
                connection.Open();
                try
                {
                    string query = "DELETE FROM Products WHERE id = @id";
                    using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        int rowAffected = command.ExecuteNonQuery();
                        if (rowAffected <= 0)
                        {
                            throw new Exception("No rows were deleted.");
                        }
                        else { return true; }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error deleting product: " + ex.Message);
                }
            }
        }
        public Products SearchProductById(int id)
        {
            using (var connection = DbConnection.Instance.GetConnection())
            {
                connection.Open();

                string query = @"SELECT p.id, p.name, p.categories_id, p.supplier_id, p.price, p.discount,p.status
                         FROM Products p
                         WHERE p.id = @id";

                using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Products
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                CategoryID = reader.GetInt32(2),
                                SupplierID = reader.GetInt32(3),
                                Price = reader.GetDecimal(4),
                                Discount = reader.GetDecimal(5),
                                Status = reader.GetString(6)
                            };
                        }
                        return null; // not found
                    }
                }
            }
        }

        public List<Products> SearchProductByName(string name)
        {
            List<Products> products = new List<Products>(); // ✅ initialize list to avoid null reference

            using (var connection = DbConnection.Instance.GetConnection())
            {
                try
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
                            s.name AS supplierName,
                            p.status
                        FROM products p
                        JOIN categories c ON p.categories_id = c.id
                        JOIN suppliers s ON p.supplier_id = s.id
                        WHERE p.status = 'active' 
                            AND (p.name LIKE '%' + @name + '%' 
                                OR CAST(p.id AS VARCHAR) LIKE '%' + @name + '%'
                                OR c.name LIKE '%' + @name + '%' 
                                OR s.name LIKE '%' + @name + '%'
                                OR CAST(p.price AS VARCHAR(10)) LIKE '%' + @name + '%')
                        ORDER BY p.id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", "" + name + "%");

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(new Products
                                {
                                    ID = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Price = reader.GetDecimal(2),
                                    Discount = reader.GetDecimal(3),
                                    CategoryID = reader.GetInt32(4),
                                    SupplierID = reader.GetInt32(5),
                                    CategoryName = reader.GetString(6),
                                    SupplierName = reader.GetString(7),
                                    Status = reader.GetString(8)
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error searching product by name: {ex.Message}");
                }
            }

            return products; // ✅ return list (even empty)
        }

    }

}
