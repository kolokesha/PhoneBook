using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

public class PersonAddRequest
{
    [Required(ErrorMessage = "Person Name can't be blank")]
    public string? PersonName { get; set; }
    
    [Required(ErrorMessage = "Person Email can't be blank")]
    [EmailAddress(ErrorMessage = "Not correct email")]
    [DataType(DataType.EmailAddress)]
    public string? PersonEmail{ get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }
    [Required(ErrorMessage = "Gender can not be blank")]
    public GenderOptions? Gender { get; set; }
    [Required(ErrorMessage = "Please select a country")]
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }

    public Person ToPersons()
    {
        return new Person()
        {
            PersonName = PersonName,
            PersonEmail = PersonEmail,
            DateOfBirth = DateOfBirth,
            Gender = Gender.ToString(),
            Address = Address,
            CountryId = CountryId,
            ReceiveNewsLetters = ReceiveNewsLetters
        };
    }
}