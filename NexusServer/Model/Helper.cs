using System.Security.Cryptography;
using System.Text.RegularExpressions;
using BCrypt.Net;

namespace NexusServer.Model
{
    public class Helper:IHelper
    {
        public string HashPassword(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return hashedPassword;
        }
        public string GenerateSessionToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[16]; // 16 bytes for a 32-character token
                rng.GetBytes(tokenData);
                return BitConverter.ToString(tokenData).Replace("-", "").ToLower();
            }
        }

        public bool IsValidEmail(string email)
        {
            string pattern = @"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        
    }


}
