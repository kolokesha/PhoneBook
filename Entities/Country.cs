﻿using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Entities;

/// <summary>
/// Domain model for Country
/// </summary>
public class Country
{
    [Key]
    public Guid CountryId { get; set; }
    public string? CountryName { get; set; }
}