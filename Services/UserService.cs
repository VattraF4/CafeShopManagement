using System;
using System.Collections.Generic;
using OOADCafeShopManagement.Interface;
using OOADCafeShopManagement.Models;
using OOADCafeShopManagement.Factory;

namespace OOADCafeShopManagement.Services
{
    /// <summary>
    /// User Service Layer - Business logic for user management
    /// Uses Repository Pattern for data access
    /// Uses Factory Pattern for object creation
    /// </summary>
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserFactory _userFactory;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _userFactory = new UserFactory();
        }

        public UserService(IUserRepository userRepository, IUserFactory userFactory)
        {
            _userRepository = userRepository;
            _userFactory = userFactory;
        }

        public List<Users> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public Users GetUserById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("User ID must be greater than zero.", nameof(id));
            }
            return _userRepository.GetUserById(id);
        }

        public Users GetUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be empty.", nameof(username));
            }
            return _userRepository.GetUserByUsername(username);
        }

        public bool AddUser(string username, string password, string role, string status, string profilePicturePath = null)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required.", nameof(username));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role is required.", nameof(role));

            if (password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters long.", nameof(password));

            // Check if username exists
            if (_userRepository.IsUsernameExists(username))
            {
                throw new Exception("Username already exists.");
            }

            // Use UserFactory to create user object
            var user = _userFactory.CreateUser(username, password, role);
            user.Status = status ?? "Active";
            user.ProfilePicturePath = profilePicturePath;

            return _userRepository.AddUser(user);
        }

        public bool UpdateUser(int id, string username, string role, string status, string profilePicturePath = null)
        {
            // Validation
            if (id <= 0)
                throw new ArgumentException("Invalid user ID.", nameof(id));
            
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required.", nameof(username));
            
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role is required.", nameof(role));

            // Get existing user
            var existingUser = _userRepository.GetUserById(id);
            if (existingUser == null)
            {
                throw new Exception("User not found.");
            }

            // Update user properties
            existingUser.Username = username;
            existingUser.Role = role;
            existingUser.Status = status ?? "Active";
            existingUser.ProfilePicturePath = profilePicturePath;

            return _userRepository.UpdateUser(existingUser);
        }

        public bool UpdateUserWithPassword(int id, string username, string password, string role, string status, string profilePicturePath = null)
        {
            // Validation
            if (id <= 0)
                throw new ArgumentException("Invalid user ID.", nameof(id));
            
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required.", nameof(username));
            
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));
            
            if (password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters long.", nameof(password));

            // Get existing user
            var existingUser = _userRepository.GetUserById(id);
            if (existingUser == null)
            {
                throw new Exception("User not found.");
            }

            // Update user properties
            existingUser.Username = username;
            existingUser.Role = role;
            existingUser.Status = status ?? "Active";
            existingUser.ProfilePicturePath = profilePicturePath;

            return _userRepository.UpdateUserWithPassword(existingUser, password);
        }

        public bool ChangePassword(int userId, string currentPassword, string newPassword)
        {
            if (newPassword.Length < 6)
                throw new ArgumentException("New password must be at least 6 characters long.", nameof(newPassword));

            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Verify current password
            if (!SecurityHelper.VerifyPassword(currentPassword, user.Password))
            {
                throw new Exception("Current password is incorrect.");
            }

            // Hash new password
            string hashedPassword = SecurityHelper.HashPassword(newPassword);
            return _userRepository.UpdateUserPassword(userId, hashedPassword);
        }

        public bool DeleteUser(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user ID.", nameof(id));

            var user = _userRepository.GetUserById(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return _userRepository.DeleteUser(id);
        }

        public bool DeactivateUser(int id)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            user.Status = "Inactive";
            return _userRepository.UpdateUser(user);
        }

        public bool ActivateUser(int id)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            user.Status = "Active";
            return _userRepository.UpdateUser(user);
        }

        public List<Users> GetActiveUsers()
        {
            var allUsers = _userRepository.GetAllUsers();
            return allUsers.FindAll(u => u.IsActive());
        }

        public List<Users> GetUsersByRole(string role)
        {
            var allUsers = _userRepository.GetAllUsers();
            return allUsers.FindAll(u => u.Role.Equals(role, StringComparison.OrdinalIgnoreCase));
        }
    }
}
