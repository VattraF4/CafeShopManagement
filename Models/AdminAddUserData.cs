using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace OOADCafeShopManagement
{
    class AdminAddUserData : DbConnection
    {
        public int ID { set; get; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public DateTime RegisterDate { get; set; }
        public string ProfilePicturePath { get; set; }

        public List<AdminAddUserData> UsersListData()
        {
            List<AdminAddUserData> UsersList = new List<AdminAddUserData>();
            using (var connection = GetConnection())
            {
                try
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    using (var command = new SqlCommand())
                    //using (var command = connection.CreateCommand())  //Same as above
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT id,username,password,role,status,reg_date FROM Users";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var userData = new AdminAddUserData
                                {
                                    // Option 1: Using column names (recommended)
                                    ID = (int)reader["id"],
                                    Username = reader["username"].ToString(),
                                    Password = reader["password"].ToString(),
                                    Role = reader["role"].ToString(),
                                    Status = reader["status"].ToString(),
                                    RegisterDate = Convert.ToDateTime(reader["reg_date"])

                                    // Option 2: Using ordinal positions
                                    // ID = reader.GetInt32(0),
                                    // Username = reader.GetString(1),
                                    // Password = reader.GetString(2),
                                    // Role = reader.GetString(3),
                                    // Status = reader.GetString(4)
                                };
                                UsersList.Add(userData);
                                //Console.WriteLine("User Added: " + userData.Username);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }

            }
            return UsersList;
        }


        //Constructor
        public AdminAddUserData() { }
        public AdminAddUserData(string username, string password, string role)
        {
            Username = username;
            Password = password;
            Role = role;
        }


    }
}
