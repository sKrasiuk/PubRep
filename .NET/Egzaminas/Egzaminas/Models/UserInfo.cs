using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Egzaminas.Models;

public class UserInfo
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserName { get; set; }

    [Required]
    [JsonIgnore]
    public string Password { get; set; }

    [JsonIgnore]
    public string Salt { get; set; }

    public string Role { get; set; } = "User";

    public int? PersonInfoId { get; set; }
    
    [JsonIgnore]
    public PersonInfo PersonInfo { get; set; }
}
