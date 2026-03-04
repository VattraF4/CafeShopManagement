using System;

namespace OOADCafeShopManagement
{
    /// <summary>
    /// User domain model - Contains only properties and business logic
    /// Data access is handled by UserRepository (Repository Pattern)
    /// </summary>
    public class Users
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public DateTime RegisterDate { get; set; }
        public string ProfilePicturePath { get; set; }

        public Users() { }

        public Users(string username, string password, string role)
        {
            Username = username;
            Password = password;
            Role = role;
            Status = "Active";
            RegisterDate = DateTime.Now;
        }

        public Users(string username, string password, string role, string status)
        {
            Username = username;
            Password = password;
            Role = role;
            Status = status;
            RegisterDate = DateTime.Now;
        }

        // Business logic methods can stay here (not data access)
        public bool IsAdmin()
        {
            return Role?.Equals("Admin", StringComparison.OrdinalIgnoreCase) ?? false;
        }

        public bool IsCashier()
        {
            return Role?.Equals("Cashier", StringComparison.OrdinalIgnoreCase) ?? false;
        }

        public bool IsActive()
        {
            return Status?.Equals("Active", StringComparison.OrdinalIgnoreCase) ?? false;
        }

        public string GetDisplayName()
        {
            return $"{Username} ({Role})";
        }
    }
}