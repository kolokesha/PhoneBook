using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

public class PersonUpdateRequest
{
    [Required]
    public Guid PersonId { get; set; }

    [Required(ErrorMessage = "Person Name can't be blank")]
    public string? PersonName { get; set; }
    [Required(ErrorMessage = "Person Email can't be blank")]
    [EmailAddress(ErrorMessage = "Not correct email")]
    public string? PersonEmail{ get; set; }
    public DateTime? DateOfBirth { get; set; }
    public GenderOptions? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }

    public Person ToPersons()
    {
        return new Person()
        {
            PersonId = PersonId,
            PersonName = PersonName,
            PersonEmail = PersonEmail,
            DateOfBirth = DateOfBirth,
            Gender = Gender.ToString(),
            CountryId = CountryId,
            Address = Address,
            ReceiveNewsLetters = ReceiveNewsLetters
        };
    }
}