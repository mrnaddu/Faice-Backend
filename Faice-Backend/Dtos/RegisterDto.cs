﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Faice_Backend.Dtos;

#nullable enable
public class RegisterDto
{
    [Required(ErrorMessage = "User Name is required")]
    public string? Username { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [PasswordPropertyText]
    public string? Password { get; set; }

    [Required(ErrorMessage = "PhoneNumber is required")]
    [Phone]
    public string? PhoneNumber { get; set; }
}
