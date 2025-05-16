using System;
using Egzaminas.Data;
using Egzaminas.Helpers;
using Egzaminas.Models;
using Microsoft.EntityFrameworkCore;

namespace Egzaminas.Services;

public class AdminService
{
    private readonly ApplicationDbContext _context;

    public AdminService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message)> DeleteUserByIdAsync(int userId)
    {
        var user = await _context.Users.Include(u => u.PersonInfo)
                    .ThenInclude(p => p.Address)
                    .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return (false, "User not found");
        }

        var (canDelete, message) = await CanDeleteAdminUserAsync(user);
        if (!canDelete)
        {
            return (false, message);
        }

        if (user.PersonInfoId.HasValue && user.PersonInfo != null)
        {
            var personInfo = user.PersonInfo;
            var address = personInfo.Address;

            _context.People.Remove(personInfo);

            if (address != null)
            {
                bool isAddressShared = await _context.People.AnyAsync(p => p.AddressInfoId == address.Id && p.Id != personInfo.Id);

                if (!isAddressShared)
                {
                    _context.Addresses.Remove(address);
                }
            }
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return (true, "User deleted successfully");
    }

    public async Task<(bool Success, string Message)> DeleteUserByPersonalNumberAsync(string personalNumber)
    {
        var person = await _context.People.Include(p => p.User)
                    .Include(p => p.Address)
                    .FirstOrDefaultAsync(p => p.PersonalNumber == personalNumber);

        if (person == null)
        {
            return (false, "Person with given personal number not found");
        }

        var user = person.User;
        var address = person.Address;

        var (canDelete, message) = await CanDeleteAdminUserAsync(user);
        if (!canDelete)
        {
            return (false, message);
        }

        _context.People.Remove(person);

        if (address != null)
        {
            bool isAddressShared = await _context.People.AnyAsync(p => p.AddressInfoId == address.Id && p.Id != person.Id);

            if (!isAddressShared)
            {
                _context.Addresses.Remove(address);
            }
        }

        if (user != null)
        {
            _context.Users.Remove(user);
        }

        await _context.SaveChangesAsync();

        return (true, "User deleted successfully");
    }

    public async Task<(bool Success, string Message)> SetUserRoleAsync(int userId, string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return (false, "Role cannot be empty.");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return (false, "User not found.");
        }

        user.Role = role;
        await _context.SaveChangesAsync();

        return (true, $"Role for user {user.UserName} set to '{role}'.");
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

    // Helper methods
    private async Task<(bool CanDelete, string? Message)> CanDeleteAdminUserAsync(UserInfo user)
    {
        if (user.Role == "Admin")
        {
            int adminCount = await _context.Users.CountAsync(u => u.Role == "Admin");

            if (adminCount <= 1)
            {
                return (false, "Cannot delete the last admin user");
            }
        }

        return (true, null);
    }
}
