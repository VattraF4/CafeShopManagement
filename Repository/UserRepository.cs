using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using OOADCafeShopManagement.Interface;
using OOADCafeShopManagement.Models;

namespace OOADCafeShopManagement.Repository
{
    public class UserRepository : IUserRepository
    {
        public List<Users> GetAllUsers()
        {
            List<Users> usersList = new List<Users>();
            try
            {
                using (var connection = DbConnection.Instance.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT id, username, password, role, status, reg_date, profile_img FROM Users";
                    
                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new Users
                            {
                                ID = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                Role = reader.GetString(3),
                                Status = reader.GetString(4),
                                RegisterDate = reader.GetDateTime(5),
                                ProfilePicturePath = reader.IsDBNull(6) ? null : reader.GetString(6)
                            };
                            usersList.Add(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving users: " + ex.Message);
                throw;
            }
            return usersList;
        }

        public Users GetUserById(int id)
        {
            try
            {
                using (var connection = DbConnection.Instance.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT id, username, password, role, status, reg_date, profile_img FROM Users WHERE id = @id";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Users
                                {
                                    ID = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    Password = reader.GetString(2),
                                    Role = reader.GetString(3),
                                    Status = reader.GetString(4),
                                    RegisterDate = reader.GetDateTime(5),
                                    ProfilePicturePath = reader.IsDBNull(6) ? null : reader.GetString(6)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting user: " + ex.Message);
                throw;
            }
            return null;
        }

        public Users GetUserByUsername(string username)
        {
            try
            {
                using (var connection = DbConnection.Instance.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT id, username, password, role, status, reg_date, profile_img FROM Users WHERE username = @username";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Users
                                {
                                    ID = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    Password = reader.GetString(2),
                                    Role = reader.GetString(3),
                                    Status = reader.GetString(4),
                                    RegisterDate = reader.GetDateTime(5),
                                    ProfilePicturePath = reader.IsDBNull(6) ? null : reader.GetString(6)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting user by username: " + ex.Message);
                throw;
            }
            return null;
        }

        public bool AddUser(Users user)
        {
            try
            {
                using (var connection = DbConnection.Instance.GetConnection())
                {
                    connection.Open();
                    
                    if (IsUsernameExists(username: user.Username))
                    {
                        throw new Exception("Username already exists.");
                    }

                    string query = @"INSERT INTO Users (username, password, role, status, reg_date, profile_img) 
                                   VALUES (@username, @password, @role, @status, @reg_date, @profile_img)";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", user.Username);
                        command.Parameters.AddWithValue("@password", user.Password);
                        command.Parameters.AddWithValue("@role", user.Role);
                        command.Parameters.AddWithValue("@status", user.Status);
                        command.Parameters.AddWithValue("@reg_date", DateTime.Now);
                        command.Parameters.AddWithValue("@profile_img", user.ProfilePicturePath ?? (object)DBNull.Value);
                        
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding user: " + ex.Message);
                throw;
            }
        }

        public bool UpdateUser(Users user)
        {
            try
            {
                using (var connection = DbConnection.Instance.GetConnection())
                {
                    connection.Open();
                    
                    if (IsUsernameExists(user.Username, user.ID))
                    {
                        throw new Exception("Username already exists.");
                    }

                    string query = @"UPDATE Users SET 
                                   username = @username, 
                                   role = @role, 
                                   status = @status, 
                                   profile_img = @profile_img 
                                   WHERE id = @id";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", user.ID);
                        command.Parameters.AddWithValue("@username", user.Username);
                        command.Parameters.AddWithValue("@role", user.Role);
                        command.Parameters.AddWithValue("@status", user.Status);
                        command.Parameters.AddWithValue("@profile_img", user.ProfilePicturePath ?? (object)DBNull.Value);
                        
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating user: " + ex.Message);
                throw;
            }
        }

        public bool UpdateUserPassword(int id, string hashedPassword)
        {
            try
            {
                using (var connection = DbConnection.Instance.GetConnection())
                {
                    connection.Open();
                    string query = "UPDATE Users SET password = @password WHERE id = @id";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@password", hashedPassword);
                        
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating user password: " + ex.Message);
                throw;
            }
        }

        public bool DeleteUser(int id)
        {
            try
            {
                using (var connection = DbConnection.Instance.GetConnection())
                {
                    connection.Open();
                    string query = "DELETE FROM Users WHERE id = @id";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting user: " + ex.Message);
                throw;
            }
        }

        public bool IsUsernameExists(string username, int excludeId = 0)
        {
            try
            {
                using (var connection = DbConnection.Instance.GetConnection())
                {
                    connection.Open();
                    string query;
                    
                    if (excludeId > 0)
                    {
                        query = "SELECT COUNT(1) FROM Users WHERE username = @username AND id != @excludeId";
                    }
                    else
                    {
                        query = "SELECT COUNT(1) FROM Users WHERE username = @username";
                    }
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        if (excludeId > 0)
                        {
                            command.Parameters.AddWithValue("@excludeId", excludeId);
                        }
                        
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking username: " + ex.Message);
                throw;
            }
        }
    }
}
