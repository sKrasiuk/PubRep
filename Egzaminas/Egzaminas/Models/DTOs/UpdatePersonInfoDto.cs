using System;
using System.ComponentModel.DataAnnotations;

namespace Egzaminas.Models.DTOs;

public class UpdatePersonInfoDto
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? PersonalNumber { get; set; }
    public string? PhoneNumber { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public IFormFile? ProfilePicture { get; set; }
    public string? City { get; set; }
    public string? StreetName { get; set; }
    public int? HouseNumber { get; set; }
    public int? FlatNumber { get; set; }
}
