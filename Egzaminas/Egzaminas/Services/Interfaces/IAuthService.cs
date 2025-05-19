using System;
using Egzaminas.Models;
using Egzaminas.Models.DTOs;

namespace Egzaminas.Services.Interfaces;

public interface IAuthService
{
    Task<UserInfo> RegisterUser(RegisterUserDto registerUserDto);
    Task<string> LoginUser(LoginDto loginDto);
    string GenerateJwtToken(UserInfo user);
}
