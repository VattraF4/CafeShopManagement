using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using OOADCafeShopManagement.Interface;
using OOADCafeShopManagement.Models;

namespace OOADCafeShopManagement.Repositories
{
    public class ProductRepository : DbConnection, IProductRepository
    {
        public List<Products> GetAllProducts()
        {
            var products = new List<Products>();

            using (var connection = GetConnection())
            {
                connection.Open();
                string query = @"SELECT
                                    p.id, p.name, p.price, p.discount, 
                                    p.categories_id, p.supplier_id, p.status,
                                    c.name AS categoryName, s.name AS supplierName
                                 FROM products p
                                 JOIN categories c ON p.categories_id = c.id
                                 JOIN suppliers s ON p.supplier_id = s.id
                                 ORDER BY p.id";

                using (var command = new SqlCommand(query, connection))
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
                            Status = reader.GetString(6),
                            CategoryName = reader.GetString(7),
                            SupplierName = reader.GetString(8)
                        });
                    }
                }
            }
            return products;
        }

        public Products GetProductById(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = @"SELECT id, name, categories_id, supplier_id, price, discount, status
                                 FROM Products
                                 WHERE id = @id";

                using (var command = new SqlCommand(query, connection))
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
                    }
                }
            }
            return null;
        }

        public List<Products> SearchProducts(string keyword)
        {
            var products = new List<Products>();

            using (var connection = GetConnection())
            {
                connection.Open();
                string query = @"SELECT
                                    p.id, p.name, p.price, p.discount,
                                    p.categories_id, p.supplier_id, c.name AS categoryName,
                                    s.name AS supplierName, p.status
                                 FROM products p
                                 JOIN categories c ON p.categories_id = c.id
                                 JOIN suppliers s ON p.supplier_id = s.id
                                 WHERE p.status = 'active'
                                    AND (p.name LIKE '%' + @keyword + '%'
                                        OR CAST(p.id AS VARCHAR) LIKE '%' + @keyword + '%'
                                        OR c.name LIKE '%' + @keyword + '%'
                                        OR s.name LIKE '%' + @keyword + '%'
                                        OR CAST(p.price AS VARCHAR(10)) LIKE '%' + @keyword + '%')
                                 ORDER BY p.id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@keyword", keyword);

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
            return products;
        }

        public bool AddProduct(Products product)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = @"INSERT INTO Products
                                 (name, categories_id, supplier_id, price, discount)
                                 VALUES (@name, @categories_id, @supplier_id, @price, @discount)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", product.Name);
                    command.Parameters.AddWithValue("@categories_id", product.CategoryID);
                    command.Parameters.AddWithValue("@supplier_id", product.SupplierID);
                    command.Parameters.AddWithValue("@price", product.Price);
                    command.Parameters.AddWithValue("@discount", product.Discount);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateProduct(Products product)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = @"UPDATE Products SET
                                    name = @name,
                                    categories_id = @categories_id,
                                    supplier_id = @supplier_id,
                                    price = @price,
                                    discount = @discount,
                                    status = @status
                                 WHERE id = @id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", product.ID);
                    command.Parameters.AddWithValue("@name", product.Name);
                    command.Parameters.AddWithValue("@categories_id", product.CategoryID);
                    command.Parameters.AddWithValue("@supplier_id", product.SupplierID);
                    command.Parameters.AddWithValue("@price", product.Price);
                    command.Parameters.AddWithValue("@discount", product.Discount);
                    command.Parameters.AddWithValue("@status", product.Status);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteProduct(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "DELETE FROM Products WHERE id = @id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
