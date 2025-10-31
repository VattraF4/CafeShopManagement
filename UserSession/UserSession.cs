using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using OOADCafeShopManagement.Db;

namespace OOADCafeShopManagement
{
    public static class UserSession
    {
        public static int UserId { get; set; }
        public static string Username { get; set; }
        public static string Role { get; set; }
        public static DateTime LoginTime { get; set; }
        public static string ProfilePath { get; set; }
        public static bool IsLoggedIn => UserId > 0;

        public static void Logout()
        {
            UserId = 0;
            Username = string.Empty;
            Role = string.Empty;
            LoginTime = DateTime.MinValue;
        }
        public static void Initialize(int userId, string username, string role, string profilePath)
        {
            UserId = userId;
            Username = username;
            Role = role;
            ProfilePath = profilePath;
            LoginTime = DateTime.Now;
        }
    }
}
