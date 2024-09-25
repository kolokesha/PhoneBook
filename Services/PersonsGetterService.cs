using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Entities;
using Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services;

public class PersonsGetterService : IPersonGetterService
{
    private readonly IPersonsRepository _personsRepository;
    private readonly ILogger<PersonsGetterService> _logger;
    private readonly IDiagnosticContext _diagnosticContext;

    public PersonsGetterService(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnosticContext)
    {
        _personsRepository = personsRepository;
        _logger = logger;
        _diagnosticContext = diagnosticContext;
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

        _diagnosticContext.Set("Persons", persons);
        
        return persons.Select(temp=> temp.ToPersonResponse()).ToList();
    }
}