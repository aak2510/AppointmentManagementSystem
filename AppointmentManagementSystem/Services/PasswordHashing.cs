using System.Security.Cryptography;
using System.Text;

namespace AppointmentManagementSystem.Services;

// Utility class for password hashing using SHA256 algorithm
public static class PasswordHashing
{
    // Method to generate SHA256 hash from a given input string (a password in this cirmcumstance
    public static string Sha256(this string password)
    {
        // Create an instance of SHA256 hash algorithm
        using (var sha = SHA256.Create())
        {
            // Convert the input string (password) to bytes using UTF8 encoding
            // Hashing works on bytes and not strings so we need to convert it here
            var bytes = Encoding.UTF8.GetBytes(password);

            // This method performs the actual computation to generate the hash value
            var hash = sha.ComputeHash(bytes);

            // Convert the hash bytes to a Base64 string
            // Base64 encoding converts binary data into ASCII characters
            // We can then return this as a string
            return Convert.ToBase64String(hash);
        }
    }
}
