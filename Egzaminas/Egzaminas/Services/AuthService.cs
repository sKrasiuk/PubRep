using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Egzaminas.Data;
using Egzaminas.Helpers;
using Egzaminas.Models;
using Egzaminas.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Egzaminas.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService( ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<UserInfo> RegisterUser(RegisterUserDto registerUserDto)
    {
        if (await _context.Users.AnyAsync(u => u.UserName == registerUserDto.UserName))
        {
            throw new InvalidOperationException("Username already exists");
        }

        var salt = PasswordHelper.GenerateSalt();
        var passwordHash = PasswordHelper.HashPassword(registerUserDto.Password, salt);

        var user = new UserInfo
        {
            UserName = registerUserDto.UserName,
            Password = passwordHash,
            Salt = Convert.ToBase64String(salt),
            Role = "User"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<string> LoginUser(LoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);

        if (user == null)
        {
            throw new InvalidOperationException("Invalid username or password");
        }

        var salt = Convert.FromBase64String(user.Salt);
        var passwordHash = PasswordHelper.HashPassword(loginDto.Password, salt);

        if (user.Password != passwordHash)
        {
            throw new InvalidProgramException("Invalid username or password");
        }

        return GenerateJwtToken(user);
    }

    public string GenerateJwtToken(UserInfo user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            };
 
            var secret = _configuration.GetSection("Jwt:Secret").Value;
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
 
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
 
            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("Jwt:Issuer").Value,
                audience: _configuration.GetSection("Jwt:Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);
 
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
}
