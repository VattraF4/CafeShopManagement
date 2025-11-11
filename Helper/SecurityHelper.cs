using System;
using System.Security.Cryptography;
using System.Text;

public static class SecurityHelper
{
    public static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower(); // Convert to hex string
        }
    }
    public static bool VerifyPassword(string enteredPassword, string storedHashedPassword)
    {
        string enteredHashed = HashPassword(enteredPassword);
        return enteredHashed == storedHashedPassword;
    }
    public static bool HashesAreEqual(string hash1, string hash2)
    {
        return StringComparer.OrdinalIgnoreCase.Compare(hash1, hash2) == 0;
    }
    public static string HashAndVerify(string password, string storedHashedPassword)
    {
        string hashedPassword = HashPassword(password);
        return HashesAreEqual(hashedPassword, storedHashedPassword) ? "Match" : "No Match";
    }

}
