using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services;

public class PersonsService : IPersonService
{
    private readonly PhoneBookDbContext _db;
    private readonly ICountriesService _countriesService;

    public PersonsService(PhoneBookDbContext dbContext, ICountriesService countriesService)
    {
        _db = dbContext;
        _countriesService = countriesService;
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
        _db.Persons.Add(person);
        await _db.SaveChangesAsync();
        
        return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        var persons = await _db.Persons.Include("Country").ToListAsync();
        return persons
            .Select(temp => temp.ToPersonResponse()).ToList();
    }

    public async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
    {
        if (personId == null)
        {
            return null;
        }

        Person? person = await _db.Persons.Include("Country")
            .FirstOrDefaultAsync(temp => temp.PersonId == personId);
        if (person == null)
            return null;

        return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
    {
        List<PersonResponse> allPersons = await GetAllPersons();
        List<PersonResponse> matchingPersons = allPersons;
        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            return matchingPersons;

        
        //To do: filter by receive letters
        switch (searchBy)
        {
            case nameof(PersonResponse.PersonName):
                matchingPersons = allPersons.Where(temp => 
                    (string.IsNullOrEmpty(temp.PersonName) || 
                     temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase))).ToList();
                break;
                
            case nameof(PersonResponse.PersonEmail):
                matchingPersons = allPersons.Where(temp => 
                    (!string.IsNullOrEmpty(temp.PersonEmail)
                        ? temp.PersonEmail.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;
            case nameof(PersonResponse.CountryId):
                matchingPersons = allPersons.Where(temp => 
                    (!string.IsNullOrEmpty(temp.Country)
                        ? temp.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                break;
            case nameof(PersonResponse.Address):
                matchingPersons = allPersons.Where(temp => 
                    (temp.Address == null) || temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                break;
            case nameof(PersonResponse.Gender):
                matchingPersons = allPersons.Where(temp => 
                    (temp.Gender == null) || temp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                break;
            case nameof(PersonResponse.DateOfBirth):
                matchingPersons = allPersons.Where(temp => 
                    (temp.DateOfBirth == null) || temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                break;
            default:
                matchingPersons = allPersons;
                break;
        }

        return matchingPersons;
    }

    public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy))
            return allPersons;
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
                    allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.Desc) => 
                    allPersons.OrderByDescending(temp => 
                        temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
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

        var matchingPerson = await _db.Persons
            .FirstOrDefaultAsync(temp => temp.PersonId == personUpdateRequest.PersonId);
        if (matchingPerson == null)
        {
            throw new ArgumentException("Giving person id does not exist");
        }

        matchingPerson.PersonName = personUpdateRequest.PersonName;
        matchingPerson.PersonEmail = personUpdateRequest.PersonEmail;
        matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
        matchingPerson.Gender = personUpdateRequest.Gender.ToString();
        matchingPerson.CountryId = personUpdateRequest.CountryId;
        matchingPerson.Address = personUpdateRequest.Address;
        matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

        _db.SaveChanges();

        return matchingPerson.ToPersonResponse();

    }

    public async Task<bool> DeletePerson(Guid? personId)
    {
        if (personId == null)
        {
            throw new ArgumentNullException(nameof(personId));
        }

        Person? person = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == personId);
        if (person == null)
        {
            return false;
        }

        _db.Persons.
            Remove(_db.Persons.
                First(temp => temp.PersonId == personId));
        await _db.SaveChangesAsync();
        return true;
    }
}