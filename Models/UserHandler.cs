using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOADCafeShopManagement.Models
{
     class UserHandler : DbConnection
    {
        public UserHandler() { }
        private string username;
        private string password;
        public string Username
        {
            get { return username; }
            set { username = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public bool Login(string username, string password)
        {
            return true;
        }

    }
}
