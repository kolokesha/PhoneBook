using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services;

public class PersonsService : IPersonService
{
    private readonly IPersonsRepository _personsRepository;
    private readonly ILogger<PersonsService> _logger;

    public PersonsService(IPersonsRepository personsRepository, ILogger<PersonsService> logger)
    {
        _personsRepository = personsRepository;
        _logger = logger;
    }

    
    //Should use Stored procedures for that?? mb bad practice in small app, nothing to perform))
    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
        
        if (personAddRequest == null)
        {
            throw new ArgumentNullException(nameof(personAddRequest));
        }
        
        ValidationHelper.ModelValidation(personAddRequest);

        Person person = personAddRequest.ToPersons();
        person.PersonId = Guid.NewGuid();
        await _personsRepository.AddPerson(person);
        
        return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        _logger.LogInformation("GetAllPersons of Person Service");
        var persons = await _personsRepository.GetAllPersons();
        return persons
            .Select(temp => temp.ToPersonResponse()).ToList();
    }

    public async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
    {
        if (personId == null)
        {
            return null;
        }

        Person? person = await _personsRepository.GetPersonByPersonId(personId.Value);
        if (person == null)
            return null;

        return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
    {
        _logger.LogInformation("GetFilteredPersons of Person Service");
        List<Person> persons = searchBy switch
        {
            //To do: filter by receive letters
            nameof(PersonResponse.PersonName) =>
                await _personsRepository.GetFilteredPersons(temp =>
                    temp.PersonName.Contains(searchString)),

            nameof(PersonResponse.PersonEmail) =>
                await _personsRepository.GetFilteredPersons(temp =>
                    temp.PersonEmail.Contains(searchString)),

            nameof(PersonResponse.DateOfBirth) =>
                await _personsRepository.GetFilteredPersons(temp =>
                    temp.DateOfBirth.Value.ToString("dd MMMM yyyy")
                        .Contains(searchString)),

            nameof(PersonResponse.CountryId) =>
                await _personsRepository.GetFilteredPersons(temp =>
                    temp.Country.CountryName.Contains(searchString)),

            nameof(PersonResponse.Address) =>
                await _personsRepository.GetFilteredPersons(temp =>
                    temp.Address.Contains(searchString)),
            
            _ => await _personsRepository.GetAllPersons()
        };

        return persons.Select(temp=> temp.ToPersonResponse()).ToList();
    }

    public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
    {
        _logger.LogInformation("GetSortedPersons of Person Service");
        
        if (string.IsNullOrEmpty(sortBy))
            return allPersons;
        var genders = Enum.GetNames(typeof(GenderOptions));
        var orderMap = new Dictionary<GenderOptions, int>() {
            { GenderOptions.Male, 1 },
            { GenderOptions.Female, 2 },
            { GenderOptions.Other, 3 }
        };
        List<PersonResponse> sortedPersons = (sortBy, sortOrder)
            switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.Asc) => 
                    allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.PersonName), SortOrderOptions.Desc) => 
                    allPersons.OrderByDescending(temp => 
                        temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.PersonEmail), SortOrderOptions.Asc) => 
                    allPersons.OrderBy(temp => temp.PersonEmail, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.PersonEmail), SortOrderOptions.Desc) => 
                    allPersons.OrderByDescending(temp => 
                        temp.PersonEmail, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.Asc) => 
                    allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.Desc) => 
                    allPersons.OrderByDescending(temp => 
                        temp.DateOfBirth).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.Asc) => 
                    allPersons.OrderBy(temp => temp.Age).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.Desc) => 
                    allPersons.OrderByDescending(temp => 
                        temp.Age).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.Asc) => 
                    allPersons.OrderBy(temp => temp.Gender).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.Desc) => 
                    allPersons.OrderByDescending(temp => temp.Gender).ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.Asc) => 
                    allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.Desc) => 
                    allPersons.OrderByDescending(temp => 
                        temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.Asc) => 
                    allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.Desc) => 
                    allPersons.OrderByDescending(temp => 
                        temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.Asc) => 
                    allPersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.Desc) => 
                    allPersons.OrderByDescending(temp => 
                        temp.ReceiveNewsLetters).ToList(),
                    
                _ => allPersons
                
            };
        return sortedPersons;
    }

    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        if (personUpdateRequest == null)
        {
            throw new ArgumentNullException(nameof(Person));
        }
        
        ValidationHelper.ModelValidation(personUpdateRequest);

        var matchingPerson = await _personsRepository.GetPersonByPersonId(personUpdateRequest.PersonId);
        if (matchingPerson == null)
        {
            throw new ArgumentException("Giving person id does not exist");
        }

        matchingPerson.PersonName = personUpdateRequest.PersonName;
        matchingPerson.PersonEmail = personUpdateRequest.PersonEmail;
        matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
        matchingPerson.GenderId = (int)personUpdateRequest.Gender;
        matchingPerson.CountryId = personUpdateRequest.CountryId;
        matchingPerson.Address = personUpdateRequest.Address;
        matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

        await _personsRepository.UpdatePerson(matchingPerson);

        return matchingPerson.ToPersonResponse();

    }

    public async Task<bool> DeletePerson(Guid? personId)
    {
        if (personId == null)
        {
            throw new ArgumentNullException(nameof(personId));
        }

        Person? person = await _personsRepository.GetPersonByPersonId(personId.Value);
        if (person == null)
        {
            return false;
        }

        await _personsRepository.DeletePersonByPersonId(personId.Value);

        return true;
    }
}