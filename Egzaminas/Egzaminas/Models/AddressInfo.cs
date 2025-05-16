using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Egzaminas.Models;

public class AddressInfo
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string StreetName { get; set; }

    [Required]
    public int HouseNumber { get; set; }

    [Required]
    public int FlatNumber { get; set; }

    [JsonIgnore]
    public List<PersonInfo> People { get; set; }
}
