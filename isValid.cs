using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOADCafeShopManagement
{
    internal class isValid
    {
        private string username;
        private string password;
        private string confirmPassword;

        public isValid() { }
        public isValid(string username, string password, string confirmPassword)
        {
            this.username = username;
            this.password = password;
            this.confirmPassword = confirmPassword;
        }

        public bool isValidInputReg(string username, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }
            if (string.IsNullOrEmpty(confirmPassword))
            {
                return false;
            }
            if (password != confirmPassword)
            {
                return false;
            }
            return true;
        }
        public string alertRegister(string username, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(username))
            {
                return "Username is required";
            }
            if (string.IsNullOrEmpty(password))
            {
                return "Password is required";
            }
            if (string.IsNullOrEmpty(confirmPassword))
            {
                return "Confirm Password is required";
            }
            if (password != confirmPassword)
            {
                return "Password don't match";
            }
            return "";
        }
        public bool isValidLogin(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("Username is required");
                return false;
            }
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Password is required");
                return false;
            }
            return true;
        }
        public string alertLogin(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                return "Username is required";
            }
            if (string.IsNullOrEmpty(password))
            {
                return "Password is required";
            }
            return "";
        }
    }
}
