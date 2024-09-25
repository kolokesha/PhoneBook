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

public class PersonsUpdaterService : IPersonUpdaterService
{
    private readonly IPersonsRepository _personsRepository;
    private readonly ILogger<PersonsUpdaterService> _logger;
    private readonly IDiagnosticContext _diagnosticContext;

    public PersonsUpdaterService(IPersonsRepository personsRepository, ILogger<PersonsUpdaterService> logger, IDiagnosticContext diagnosticContext)
    {
        _personsRepository = personsRepository;
        _logger = logger;
        _diagnosticContext = diagnosticContext;
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
            throw new InvalidPersonIdException("Giving person id does not exist");
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

}