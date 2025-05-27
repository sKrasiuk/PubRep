using System;

namespace Egzaminas.Services.Interfaces;

public interface IUserPasswordService
{
    Task<(bool Success, string Message)> ChangeUserPasswordAsync(int userId, string newPassword);
}
