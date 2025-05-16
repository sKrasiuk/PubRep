using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Egzaminas.Models;

public class PersonInfo
{
    [Key]
    public int Id { get; set; }

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
    public byte[] ProfilePicture { get; set; }

    public int AddressInfoId { get; set; }

    public AddressInfo Address { get; set; }

    [JsonIgnore]
    public UserInfo User { get; set; }
}
