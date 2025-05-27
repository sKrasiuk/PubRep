using System;
using Egzaminas.Models;
using Egzaminas.Models.DTOs;

namespace Egzaminas.Services.Interfaces;

public interface IPersonService
{
    Task<PersonInfo> GetPersonInfo(int userId);
    Task<PersonInfo> AddPersonInfo(int userId, PersonInfoDto personDto);
    Task UpdatePersonInfo(int userId, UpdatePersonInfoDto updateDto);
    Task<(bool Success, string Message)> ChangeUserPasswordAsync(int userId, string newPassword);
}
