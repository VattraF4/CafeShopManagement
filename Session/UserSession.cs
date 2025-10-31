using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOADCafeShopManagement.Session
{
    public class UserSession
    {
        public static int UserId { get; set; }
        public static string Username { get; set; }
        public static string Role { get; set; }
        public static DateTime LoginTime { get; set; }
        public static bool IsLoggedIn => UserId > 0;
        public static string ProfilePicturePath { get; set; }

        public static void Logout()
        {
            UserId = 0;
            Username = string.Empty;
            Role = string.Empty;
            LoginTime = DateTime.MinValue;
        }
    }
}
