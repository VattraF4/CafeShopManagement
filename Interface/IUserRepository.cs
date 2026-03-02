using System.Collections.Generic;
using OOADCafeShopManagement.Models;

namespace OOADCafeShopManagement.Interface
{
    public interface IUserRepository
    {
        List<Users> GetAllUsers();
        Users GetUserById(int id);
        bool AddUser(Users user);
        bool UpdateUser(Users user);
        bool UpdateUserPassword(int id, string hashedPassword);
        bool DeleteUser(int id);
        Users GetUserByUsername(string username);
        bool IsUsernameExists(string username, int excludeId = 0);
    }
}
