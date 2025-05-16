using System;
using System.ComponentModel.DataAnnotations;

namespace Egzaminas.Models.DTOs;

public class RegisterUserDto
{
    [Required]
    public string UserName { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}
