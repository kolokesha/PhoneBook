using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

public class PersonResponse
{
    public Guid PersonId { get; set; }
    public string? PersonName { get; set; }
    public string? PersonEmail { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Country { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
    public double? Age { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj.GetType() != typeof(PersonResponse))
        {
            return false;
        }

        PersonResponse person = (PersonResponse)obj;
        return this.PersonId == person.PersonId && PersonName == person.PersonName && PersonEmail == person.PersonEmail
               && DateOfBirth == person.DateOfBirth && Gender == person.Gender && CountryId == person.CountryId
               && Address == person.Address && ReceiveNewsLetters == person.ReceiveNewsLetters;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"" +
               $"Person ID: {PersonId}, Person Name: {PersonName}, Person Email: {PersonEmail}," +
               $" DateOfBirth: {DateOfBirth}, Gender: {Gender}, Country ID {CountryId}, Address: {Address}," +
               $" Receive News: {ReceiveNewsLetters} ";

    }

    public PersonUpdateRequest ToPersonUpdateRequest()
    {
        return new PersonUpdateRequest()
        {
            PersonId = PersonId,
            PersonName = PersonName,
            PersonEmail = PersonEmail,
            DateOfBirth = DateOfBirth,
            Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true),
            Address = Address,
            CountryId = CountryId,
            ReceiveNewsLetters = ReceiveNewsLetters,
        };
    }
}

public static class PersonExtensions
{
    public static PersonResponse ToPersonResponse(this Person person)
    {
        return new PersonResponse()
        {
            PersonId = person.PersonId,
            PersonName = person.PersonName,
            PersonEmail = person.PersonEmail,
            DateOfBirth = person.DateOfBirth,
            Gender = person.Gender,
            Address = person.Address,
            CountryId = person.CountryId,
            Age = (person.DateOfBirth != null)
                ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365)
                : null,
            ReceiveNewsLetters = person.ReceiveNewsLetters,
            Country = person.Country?.CountryName
        };
    }
}