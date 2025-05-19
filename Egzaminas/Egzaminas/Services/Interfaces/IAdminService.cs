using System;

namespace Egzaminas.Services.Interfaces;

public interface IAdminService
{
    Task<(bool Success, string Message)> DeleteUserByIdAsync(int userId);
    Task<(bool Success, string Message)> DeleteUserByPersonalNumberAsync(string personalNumber);
    Task<(bool Success, string Message)> SetUserRoleAsync(int userId, string role);
    Task<(bool Success, string Message)> ChangeUserPasswordAsync(int userId, string newPassword);
}
