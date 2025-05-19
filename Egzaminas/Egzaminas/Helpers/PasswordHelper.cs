using System;
using System.Security.Cryptography;
using Egzaminas.Data;

namespace Egzaminas.Helpers;

public static class PasswordHelper
{
    public static byte[] GenerateSalt()
    {
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    public static string HashPassword(string password, byte[] salt)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
        {
            var hash = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(hash);
        }
    }
}
