using System;
using Egzaminas.Data;
using Egzaminas.Helpers;
using Egzaminas.Models;
using Egzaminas.Models.DTOs;
using Egzaminas.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Egzaminas.Services;

public class PersonService : IPersonService
{
    private readonly ApplicationDbContext _context;
    private readonly IImageService _imageService;
    private readonly IUserPasswordService _passwordService;

    public PersonService(ApplicationDbContext context, IImageService imageService, IUserPasswordService passwordService)
    {
        _context = context;
        _imageService = imageService;
        _passwordService = passwordService;
    }

    private void UpdatePersonInfoFields(PersonInfo person, AddressInfo address, UpdatePersonInfoDto dto, byte[] profilePictureData)
    {
        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            person.Name = dto.Name;
        }
        if (!string.IsNullOrWhiteSpace(dto.Surname))
        {
            person.Surname = dto.Surname;
        }
        if (!string.IsNullOrWhiteSpace(dto.PersonalNumber))
        {
            person.PersonalNumber = dto.PersonalNumber;
        }
        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            person.PhoneNumber = dto.PhoneNumber;
        }
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            person.Email = dto.Email;
        }
        if (profilePictureData != null)
        {
            person.ProfilePicture = profilePictureData;
        }

        if (address != null)
        {
            if (!string.IsNullOrWhiteSpace(dto.City))
            {
                address.City = dto.City;
            }
            if (!string.IsNullOrWhiteSpace(dto.StreetName))
            {
                address.StreetName = dto.StreetName;
            }
            if (dto.HouseNumber.HasValue && dto.HouseNumber.Value > 0)
            {
                address.HouseNumber = dto.HouseNumber.Value;
            }
            if (dto.FlatNumber.HasValue && dto.FlatNumber.Value > 0)
            {
                address.FlatNumber = dto.FlatNumber.Value;
            }
        }
    }

    public async Task<PersonInfo> GetPersonInfo(int userId)
    {
        var user = await _context.Users.Include(u => u.PersonInfo)
            .ThenInclude(p => p.Address)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || !user.PersonInfoId.HasValue)
        {
            throw new InvalidOperationException("Person not found");
        }

        return user.PersonInfo;
    }

    public async Task<PersonInfo> AddPersonInfo(int userId, PersonInfoDto personDto)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        if (user.PersonInfoId.HasValue)
        {
            throw new InvalidOperationException("User already has personal information");
        }

        var profilePictureData = await _imageService.ProcessProfilePicture(personDto.ProfilePicture);

        var existingAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.City == personDto.City &&
            a.StreetName == personDto.StreetName &&
            a.HouseNumber == personDto.HouseNumber &&
            a.FlatNumber == personDto.FlatNumber);

        AddressInfo address;
        if (existingAddress != null)
        {
            address = existingAddress;
        }
        else
        {
            address = new AddressInfo
            {
                City = personDto.City,
                StreetName = personDto.StreetName,
                HouseNumber = personDto.HouseNumber,
                FlatNumber = personDto.FlatNumber,
            };
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
        }

        var personInfo = new PersonInfo
        {
            Id = userId, // Shared PK with UserInfo
            Name = personDto.Name,
            Surname = personDto.Surname,
            PersonalNumber = personDto.PersonalNumber,
            PhoneNumber = personDto.PhoneNumber,
            Email = personDto.Email,
            ProfilePicture = profilePictureData,
            AddressInfoId = address.Id,
        };

        _context.People.Add(personInfo);
        await _context.SaveChangesAsync();

        user.PersonInfoId = personInfo.Id;
        await _context.SaveChangesAsync();

        return personInfo;
    }

    public async Task UpdatePersonInfo(int userId, UpdatePersonInfoDto updateDto)
    {
        var user = await _context.Users.Include(u => u.PersonInfo)
            .ThenInclude(p => p.Address)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.PersonInfo == null)
        {
            throw new InvalidOperationException("Person not found");
        }

        var person = user.PersonInfo;
        var previousAddressId = person.AddressInfoId;
        var address = person.Address;

        byte[] profilePicture = null;
        if (updateDto.ProfilePicture != null)
        {
            profilePicture = await _imageService.ProcessProfilePicture(updateDto.ProfilePicture);
        }

        bool addressChanged =
            !string.IsNullOrWhiteSpace(updateDto.City) ||
            !string.IsNullOrWhiteSpace(updateDto.StreetName) ||
            (updateDto.HouseNumber.HasValue && updateDto.HouseNumber.Value > 0) ||
            (updateDto.FlatNumber.HasValue && updateDto.FlatNumber.Value > 0);

        if (addressChanged)
        {
            string newCity = !string.IsNullOrWhiteSpace(updateDto.City) ? updateDto.City : address.City;
            string newStreet = !string.IsNullOrWhiteSpace(updateDto.StreetName) ? updateDto.StreetName : address.StreetName;
            int newHouseNumber = updateDto.HouseNumber ?? address.HouseNumber;
            int newFlatNumber = updateDto.FlatNumber ?? address.FlatNumber;

            var existingAddress = await _context.Addresses.FirstOrDefaultAsync(a =>
                a.City == newCity &&
                a.StreetName == newStreet &&
                a.HouseNumber == newHouseNumber &&
                a.FlatNumber == newFlatNumber);

            int sharers = await _context.People.CountAsync(p => p.AddressInfoId == address.Id);

            if (existingAddress != null)
            {
                if (existingAddress.Id != address.Id)
                {
                    person.AddressInfoId = existingAddress.Id;
                    person.Address = existingAddress;
                }
                UpdatePersonInfoFields(person, null, updateDto, profilePicture);
            }
            else if (sharers > 1)
            {
                var newAddress = new AddressInfo
                {
                    City = newCity,
                    StreetName = newStreet,
                    HouseNumber = newHouseNumber,
                    FlatNumber = newFlatNumber
                };
                _context.Addresses.Add(newAddress);
                await _context.SaveChangesAsync();

                person.AddressInfoId = newAddress.Id;
                person.Address = newAddress;
                UpdatePersonInfoFields(person, null, updateDto, profilePicture);
            }
            else
            {
                UpdatePersonInfoFields(person, address, updateDto, profilePicture);
            }
        }
        else
        {
            UpdatePersonInfoFields(person, address, updateDto, profilePicture);
        }

        await _context.SaveChangesAsync();

        if (addressChanged && previousAddressId != person.AddressInfoId)
        {
            bool isOrphan = !await _context.People.AnyAsync(p => p.AddressInfoId == previousAddressId);
            if (isOrphan)
            {
                var orphanAddress = await _context.Addresses.FindAsync(previousAddressId);
                if (orphanAddress != null)
                {
                    _context.Addresses.Remove(orphanAddress);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }

    public Task<(bool Success, string Message)> ChangeUserPasswordAsync(int userId, string newPassword)
    {
        return _passwordService.ChangeUserPasswordAsync(userId, newPassword);
    }

    // public async Task<PersonInfo> UpdateName(int userId, string name)
    // {
    //     if (string.IsNullOrWhiteSpace(name))
    //     {
    //         throw new InvalidOperationException("Name cannot be empty");
    //     }

    //     var person = await GetPersonInfoByUserId(userId);
    //     person.Name = name;
    //     await _context.SaveChangesAsync();
    //     return person;
    // }

    // public async Task<PersonInfo> UpdateSurname(int userId, string surname)
    // {
    //     if (string.IsNullOrWhiteSpace(surname))
    //     {
    //         throw new InvalidOperationException("Surname cannot be empty");
    //     }

    //     var person = await GetPersonInfoByUserId(userId);
    //     person.Surname = surname;
    //     await _context.SaveChangesAsync();
    //     return person;
    // }

    // public async Task<PersonInfo> UpdatePersonalNumber(int userId, string personalNumber)
    // {
    //     if (string.IsNullOrWhiteSpace(personalNumber))
    //     {
    //         throw new InvalidOperationException("Personal number cannot be empty");
    //     }

    //     var person = await GetPersonInfoByUserId(userId);
    //     person.PersonalNumber = personalNumber;
    //     await _context.SaveChangesAsync();
    //     return person;
    // }

    // public async Task<PersonInfo> UpdatePhoneNumber(int userId, string phoneNumber)
    // {
    //     if (string.IsNullOrWhiteSpace(phoneNumber))
    //     {
    //         throw new InvalidOperationException("Phone number cannot be empty");
    //     }

    //     var person = await GetPersonInfoByUserId(userId);
    //     person.PhoneNumber = phoneNumber;
    //     await _context.SaveChangesAsync();
    //     return person;
    // }

    // public async Task<PersonInfo> UpdateEmail(int userId, string email)
    // {
    //     if (string.IsNullOrWhiteSpace(email))
    //     {
    //         throw new InvalidOperationException("Email cannot be empty");
    //     }

    //     var person = await GetPersonInfoByUserId(userId);
    //     person.Email = email;
    //     await _context.SaveChangesAsync();
    //     return person;
    // }

    // public async Task<PersonInfo> UpdateCity(int userId, string city)
    // {
    //     if (string.IsNullOrWhiteSpace(city))
    //     {
    //         throw new InvalidOperationException("City cannot be empty");
    //     }

    //     var person = await GetPersonInfoByUserId(userId);
    //     person.Address.City = city;
    //     await _context.SaveChangesAsync();
    //     return person;
    // }

    // public async Task<PersonInfo> UpdateStreet(int userId, string street)
    // {
    //     if (string.IsNullOrWhiteSpace(street))
    //     {
    //         throw new InvalidOperationException("Street cannot be empty");
    //     }

    //     var person = await GetPersonInfoByUserId(userId);
    //     person.Address.StreetName = street;
    //     await _context.SaveChangesAsync();
    //     return person;
    // }

    // public async Task<PersonInfo> UpdateHouseNumber(int userId, int houseNumber)
    // {
    //     if (houseNumber <= 0)
    //     {
    //         throw new InvalidOperationException("House number must be a positive integer");
    //     }

    //     var person = await GetPersonInfoByUserId(userId);
    //     person.Address.HouseNumber = houseNumber;
    //     await _context.SaveChangesAsync();
    //     return person;
    // }

    // public async Task<PersonInfo> UpdateFlatNumber(int userId, int flatNumber)
    // {
    //     if (flatNumber <= 0)
    //     {
    //         throw new InvalidOperationException("Flat number must be a positive integer");
    //     }

    //     var person = await GetPersonInfoByUserId(userId);
    //     person.Address.FlatNumber = flatNumber;
    //     await _context.SaveChangesAsync();
    //     return person;
    // }

    // private async Task<PersonInfo> GetPersonInfoByUserId(int userId)
    // {
    //     var user = await _context.Users.Include(u => u.PersonInfo)
    //         .ThenInclude(p => p.Address)
    //         .FirstOrDefaultAsync(u => u.Id == userId);

    //     if (user == null || !user.PersonInfoId.HasValue)
    //     {
    //         throw new InvalidOperationException("Person not found");
    //     }

    //     return user.PersonInfo;
    // }
}
