using System;
using Egzaminas.Data;
using Egzaminas.Helpers;
using Egzaminas.Services.Interfaces;

namespace Egzaminas.Services;

public class UserPasswordService : IUserPasswordService
{
    private readonly ApplicationDbContext _context;

    public UserPasswordService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message)> ChangeUserPasswordAsync(int userId, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
        {
            return (false, "Password must be at least 6 characters long.");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return (false, "User not found.");
        }

        var salt = PasswordHelper.GenerateSalt();
        var passwordHash = PasswordHelper.HashPassword(newPassword, salt);

        user.Password = passwordHash;
        user.Salt = Convert.ToBase64String(salt);

        await _context.SaveChangesAsync();

        return (true, $"Password for user '{user.UserName}' has been changed.");
    }
}
