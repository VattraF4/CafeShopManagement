using System;
using OOADCafeShopManagement.Models;

namespace OOADCafeShopManagement.Factory
{
    /// <summary>
    /// Factory Pattern for creating User objects
    /// Handles different user types with specific configurations
    /// </summary>
    public interface IUserFactory
    {
        Users CreateUser(string username, string password, string role);
        Users CreateAdmin(string username, string password);
        Users CreateCashier(string username, string password);
        Users CreateManager(string username, string password);
        Users CreateStaff(string username, string password);
    }

    public class UserFactory : IUserFactory
    {
        /// <summary>
        /// Creates a user with the specified role
        /// </summary>
        public Users CreateUser(string username, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required.", nameof(username));
            
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));
            
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role is required.", nameof(role));

            // Normalize role
            role = role.ToLower().Trim();

            // Create user based on role
            switch (role)
            {
                case "admin":
                    return CreateAdmin(username, password);
                
                case "cashier":
                    return CreateCashier(username, password);
                
                case "manager":
                    return CreateManager(username, password);
                
                case "staff":
                    return CreateStaff(username, password);
                
                default:
                    throw new ArgumentException($"Invalid role: {role}. Valid roles are: admin, cashier, manager, staff.");
            }
        }

        /// <summary>
        /// Creates an Admin user with full permissions
        /// </summary>
        public Users CreateAdmin(string username, string password)
        {
            return new Users
            {
                Username = username,
                Password = password,
                Role = "admin",
                Status = "Active",
                RegisterDate = DateTime.Now
            };
        }

        /// <summary>
        /// Creates a Cashier user for POS operations
        /// </summary>
        public Users CreateCashier(string username, string password)
        {
            return new Users
            {
                Username = username,
                Password = password,
                Role = "cashier",
                Status = "Active",
                RegisterDate = DateTime.Now
            };
        }

        /// <summary>
        /// Creates a Manager user for management operations
        /// </summary>
        public Users CreateManager(string username, string password)
        {
            return new Users
            {
                Username = username,
                Password = password,
                Role = "manager",
                Status = "Active",
                RegisterDate = DateTime.Now
            };
        }

        /// <summary>
        /// Creates a Staff user for basic operations
        /// </summary>
        public Users CreateStaff(string username, string password)
        {
            return new Users
            {
                Username = username,
                Password = password,
                Role = "staff",
                Status = "Active",
                RegisterDate = DateTime.Now
            };
        }

        /// <summary>
        /// Creates a user from existing data (for database loading)
        /// </summary>
        public Users CreateFromData(int id, string username, string hashedPassword, 
            string role, string status, DateTime registerDate, string profilePicturePath = null)
        {
            return new Users
            {
                ID = id,
                Username = username,
                Password = hashedPassword,
                Role = role,
                Status = status,
                RegisterDate = registerDate,
                ProfilePicturePath = profilePicturePath
            };
        }
    }
}
