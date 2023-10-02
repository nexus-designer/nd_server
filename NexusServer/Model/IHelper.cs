using System.Security.Cryptography;
using System.Text.RegularExpressions;
using BCrypt.Net;

namespace NexusServer.Model
{
    public interface IHelper
    {
        string HashPassword(string password);

        string GenerateSessionToken();

        bool IsValidEmail(string email);
    }
}
