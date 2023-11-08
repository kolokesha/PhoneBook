using System.ComponentModel.DataAnnotations;

namespace Entities;

public class Gender
{
    [Key]
    public int GenderId { get; set; }
    public string? GenderName { get; set; }
}