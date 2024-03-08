﻿using Faice_Backend.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Faice_Backend.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public Role Role { get; set; }

    [JsonIgnore]
    public string PasswordHash { get; set; }
}