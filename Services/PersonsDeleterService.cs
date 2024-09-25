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

public class PersonsDeleterService : IPersonDeleterService
{
    private readonly IPersonsRepository _personsRepository;
    private readonly ILogger<PersonsDeleterService> _logger;
    private readonly IDiagnosticContext _diagnosticContext;

    public PersonsDeleterService(IPersonsRepository personsRepository, ILogger<PersonsDeleterService> logger, IDiagnosticContext diagnosticContext)
    {
        _personsRepository = personsRepository;
        _logger = logger;
        _diagnosticContext = diagnosticContext;
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