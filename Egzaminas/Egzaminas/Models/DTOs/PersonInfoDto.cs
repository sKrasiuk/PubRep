using System;
using System.ComponentModel.DataAnnotations;

namespace Egzaminas.Models.DTOs;

public class PersonInfoDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Surname { get; set; }

    [Required]
    public string PersonalNumber { get; set; }

    [Required]
    public string PhoneNumber { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public IFormFile ProfilePicture { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string StreetName { get; set; }

    [Required]
    public int HouseNumber { get; set; }

    [Required]
    public int FlatNumber { get; set; }
}
