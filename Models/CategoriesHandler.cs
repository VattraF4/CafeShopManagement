using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOADCafeShopManagement.Models
{
    class CategoriesHandler : DbConnection
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<CategoriesHandler> ListData()
        {
            List<CategoriesHandler> categoriesList = new List<CategoriesHandler>();
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT id, name FROM Categories Order By name";
                using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CategoriesHandler category = new CategoriesHandler
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            };
                            categoriesList.Add(category);
                        }
                    }
                }
            }
            return categoriesList;
        }
    }
}
