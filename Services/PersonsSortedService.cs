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

public class PersonsSortedService : IPersonSortedService
{
    private readonly IPersonsRepository _personsRepository;
    private readonly ILogger<PersonsSortedService> _logger;
    private readonly IDiagnosticContext _diagnosticContext;

    public PersonsSortedService(IPersonsRepository personsRepository, ILogger<PersonsSortedService> logger, IDiagnosticContext diagnosticContext)
    {
        _personsRepository = personsRepository;
        _logger = logger;
        _diagnosticContext = diagnosticContext;
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
    
}