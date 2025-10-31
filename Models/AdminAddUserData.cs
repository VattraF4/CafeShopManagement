using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

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

        // Constructors
        public AdminAddUserData() { }

        public AdminAddUserData(string username, string password, string role)
        {
            Username = username;
            Password = password;
            Role = role;
        }

        public List<AdminAddUserData> UsersListData()
        {
            List<AdminAddUserData> UsersList = new List<AdminAddUserData>();

            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT id, username, password, role, status, reg_date, profile_img FROM Users";

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var userData = new AdminAddUserData
                                {
                                    ID = (int)reader["id"],
                                    Username = reader["username"].ToString(),
                                    Password = reader["password"].ToString(),
                                    Role = reader["role"].ToString(),
                                    Status = reader["status"].ToString(),
                                    RegisterDate = Convert.ToDateTime(reader["reg_date"]),
                                    ProfilePicturePath = reader["profile_img"] != DBNull.Value ? reader["profile_img"].ToString() : null
                                };
                                UsersList.Add(userData);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error retrieving users: " + ex.Message);
                    throw;
                }
            }
            return UsersList;
        }

        public bool AddUser(string username, string password, string role, string status, string profilePicturePath = null)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();

                    if (IsUsernameExists(connection, username))
                    {
                        throw new Exception("Username already exists.");
                    }

                    string hashedPassword = SecurityHelper.HashPassword(password);

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = @"INSERT INTO Users (username, password, role, status, reg_date, profile_img) 
                                      VALUES (@username, @password, @role, @status, @reg_date, @profile_img)";

                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", hashedPassword);
                        command.Parameters.AddWithValue("@role", role);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@reg_date", DateTime.Now);

                        if (!string.IsNullOrEmpty(profilePicturePath))
                            command.Parameters.AddWithValue("@profile_img", profilePicturePath);
                        else
                            command.Parameters.AddWithValue("@profile_img", DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding user: {ex.Message}");
                    throw;
                }
            }
        }

        public bool UpdateUser(int id, string username, string role, string status, string profilePicturePath = null)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();

                    // Check if username exists (excluding current user)
                    if (IsUsernameExists(connection, username, id))
                    {
                        throw new Exception("Username already exists.");
                    }

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = @"UPDATE Users 
                                              SET username = @username, role = @role, status = @status, profile_img = @profile_img
                                              WHERE id = @id";

                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@role", role);
                        command.Parameters.AddWithValue("@status", status);

                        if (!string.IsNullOrEmpty(profilePicturePath))
                            command.Parameters.AddWithValue("@profile_img", profilePicturePath);
                        else
                            command.Parameters.AddWithValue("@profile_img", DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating user: {ex.Message}");
                    throw;
                }
            }
        }

        public bool UpdateUserWithPassword(int id, string username, string password, string role, string status, string profilePicturePath = null)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();

                    if (IsUsernameExists(connection, username, id))
                    {
                        throw new Exception("Username already exists.");
                    }

                    string hashedPassword = SecurityHelper.HashPassword(password);

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = @"UPDATE Users 
                                              SET username = @username, password = @password, role = @role, status = @status, profile_img = @profile_img
                                              WHERE id = @id";

                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", hashedPassword);
                        command.Parameters.AddWithValue("@role", role);
                        command.Parameters.AddWithValue("@status", status);

                        if (!string.IsNullOrEmpty(profilePicturePath))
                            command.Parameters.AddWithValue("@profile_img", profilePicturePath);
                        else
                            command.Parameters.AddWithValue("@profile_img", DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating user with password: {ex.Message}");
                    throw;
                }
            }
        }

        public bool DeleteUser(int id)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "DELETE FROM Users WHERE id = @id";
                        command.Parameters.AddWithValue("@id", id);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting user: {ex.Message}");
                    throw;
                }
            }
        }

        public AdminAddUserData GetUserById(int id)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT id, username, password, role, status, reg_date, profile_img FROM Users WHERE id = @id";
                        command.Parameters.AddWithValue("@id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new AdminAddUserData
                                {
                                    ID = (int)reader["id"],
                                    Username = reader["username"].ToString(),
                                    Password = reader["password"].ToString(),
                                    Role = reader["role"].ToString(),
                                    Status = reader["status"].ToString(),
                                    RegisterDate = Convert.ToDateTime(reader["reg_date"]),
                                    ProfilePicturePath = reader["profile_img"] != DBNull.Value ? reader["profile_img"].ToString() : null
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting user: {ex.Message}");
                    throw;
                }
            }
            return null;
        }

        private bool IsUsernameExists(SqlConnection connection, string username, int excludeId = 0)
        {
            using (var command = new SqlCommand())
            {
                command.Connection = connection;

                if (excludeId > 0)
                {
                    command.CommandText = "SELECT COUNT(1) FROM Users WHERE username = @username AND id != @excludeId";
                    command.Parameters.AddWithValue("@excludeId", excludeId);
                }
                else
                {
                    command.CommandText = "SELECT COUNT(1) FROM Users WHERE username = @username";
                }

                command.Parameters.AddWithValue("@username", username);

                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }
    }
}