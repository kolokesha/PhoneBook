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

public class PersonsAdderService : IPersonAdderService
{
    private readonly IPersonsRepository _personsRepository;
    private readonly ILogger<PersonsAdderService> _logger;
    private readonly IDiagnosticContext _diagnosticContext;

    public PersonsAdderService(IPersonsRepository personsRepository, ILogger<PersonsAdderService> logger, IDiagnosticContext diagnosticContext)
    {
        _personsRepository = personsRepository;
        _logger = logger;
        _diagnosticContext = diagnosticContext;
    }
    
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
}