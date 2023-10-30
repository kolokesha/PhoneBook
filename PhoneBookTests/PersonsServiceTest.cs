using Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace PhoneBookTests;

public class PersonsServiceTest
{
    private readonly IPersonService _personService;
    private readonly ICountriesService _countriesService;
    private readonly ITestOutputHelper _testOutputHelper;

    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
        _countriesService = new CountriesService(new PhoneBookDbContext(
            new DbContextOptionsBuilder<PhoneBookDbContext>().Options));
        _personService = new PersonsService(new PhoneBookDbContext(
            new DbContextOptionsBuilder<PhoneBookDbContext>().Options), _countriesService);
        
        _testOutputHelper = testOutputHelper;
    }

    #region AddPerson
    
    [Fact]
    public async Task AddPerson_NullPerson()
    {
        PersonAddRequest personAddRequest = null;
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _personService.AddPerson(personAddRequest));
    }
    
    [Fact]
    public async Task AddPerson_PersonNameNull()
    {
        PersonAddRequest personAddRequest = new PersonAddRequest() {PersonName = null};
        await Assert.ThrowsAsync<ArgumentException>(async () => await _personService.AddPerson(personAddRequest));
    }
    
    [Fact]
    public async Task AddPerson_ProperPersonDetails()
    {
        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            PersonName = "alex",
            PersonEmail = "asd@mail.ru",
            Address = "some address",
            CountryId = Guid.NewGuid(),
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Parse("2000-01-01")
        };

        PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
        List<PersonResponse> persons = await _personService.GetAllPersons();
        
        Assert.True(personResponse.PersonId != Guid.Empty);
        Assert.Contains(personResponse, persons);
    }

    #endregion

    #region GetPersonByPersonId

    [Fact]
    public async Task GetPersonByPersonId_NullPersonId()
    {
        Guid? personId = null;
        PersonResponse? personResponse = await _personService.GetPersonByPersonId(personId);
        Assert.Null(personResponse);
    }
    
    [Fact]
    public async Task GetPersonByPersonId_ProperPersonId()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest(){CountryName = "CANADA"};
        CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            PersonName = "alex",
            PersonEmail = "asd@mail.ru",
            Address = "aasd my address is",
            CountryId = countryResponse.CountryId,
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Parse("2000-01-01")
        };
        PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
        PersonResponse? personResponseFromGet = await _personService.GetPersonByPersonId(personResponse.PersonId);
        Assert.Equal(personResponse, personResponseFromGet);
    }

    #endregion

    #region GetAllPersons

    [Fact]
    public async Task GetAllPersons_EmptyList()
    {
        List<PersonResponse> persons = await _personService.GetAllPersons();
        Assert.Empty(persons);
    }
    
    [Fact]
    public async Task GetAllPersons_GetAllOfThePersons()
    {
        CountryAddRequest countryAddRequest1 = new CountryAddRequest(){CountryName = "CANADA1"};
        CountryAddRequest countryAddRequest2 = new CountryAddRequest(){CountryName = "CANADA2"};
        CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
        CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);
        
        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            PersonName = "alex",
            PersonEmail = "asd@mail.ru",
            Address = "aasd my address is",
            CountryId = countryResponse1.CountryId,
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Parse("2000-01-01")
        };
        PersonAddRequest personAddRequest2 = new PersonAddRequest()
        {
            PersonName = "nick",
            PersonEmail = "asd@mail.ru",
            Address = "aasd my address is",
            CountryId = countryResponse2.CountryId,
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Parse("2000-01-01")
        };
        
        List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
        {
            personAddRequest1,
            personAddRequest2
        };
        List<PersonResponse> personResponses = new List<PersonResponse>();

        foreach (var variablePersonAddRequest in personAddRequests)
        {
            PersonResponse personResponse = await _personService.AddPerson(variablePersonAddRequest);
            personResponses.Add(personResponse);
        }
        
        _testOutputHelper.WriteLine("Expected:");
        foreach (var variablePersonResponse in personResponses)
        {
            _testOutputHelper.WriteLine(variablePersonResponse.ToString());
        }

        List<PersonResponse> personResponsesFromGetAllPersons = await _personService.GetAllPersons();
        
        _testOutputHelper.WriteLine("Actual:");
        foreach (var variablePersonResponse in personResponsesFromGetAllPersons)
        {
            _testOutputHelper.WriteLine(variablePersonResponse.ToString());
        }

        foreach (var personResponse in personResponses)
        {
            Assert.Contains(personResponse, personResponsesFromGetAllPersons);
        }
    }

    #endregion

    #region GetFilteredPersons

    
    
    [Fact]
    public async Task GetFilteredPersons_EmptySearchText()
    {
        CountryAddRequest countryAddRequest1 = new CountryAddRequest(){CountryName = "CANADA1"};
        CountryAddRequest countryAddRequest2 = new CountryAddRequest(){CountryName = "CANADA2"};
        CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
        CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);
        
        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            PersonName = "alex",
            PersonEmail = "asd@mail.ru",
            Address = "aasd my address is",
            CountryId = countryResponse1.CountryId,
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Parse("2000-01-01")
        };
        PersonAddRequest personAddRequest2 = new PersonAddRequest()
        {
            PersonName = "nick",
            PersonEmail = "asd@mail.ru",
            Address = "aasd my address is",
            CountryId = countryResponse2.CountryId,
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Parse("2000-01-01")
        };
        
        List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
        {
            personAddRequest1,
            personAddRequest2
        };
        List<PersonResponse> personResponses = new List<PersonResponse>();

        foreach (var variablePersonAddRequest in personAddRequests)
        {
            PersonResponse personResponse = await _personService.AddPerson(variablePersonAddRequest);
            personResponses.Add(personResponse);
        }
        
        _testOutputHelper.WriteLine("Expected:");
        foreach (var variablePersonResponse in personResponses)
        {
            _testOutputHelper.WriteLine(variablePersonResponse.ToString());
        }

        List<PersonResponse> personResponsesFromSearch = 
            await _personService.GetFilteredPersons(nameof(Person.PersonName), "");
        
        _testOutputHelper.WriteLine("Actual:");
        foreach (var variablePersonResponse in personResponsesFromSearch)
        {
            _testOutputHelper.WriteLine(variablePersonResponse.ToString());
        }

        foreach (var personResponse in personResponses)
        {
            Assert.Contains(personResponse, personResponsesFromSearch);
        }
    }
    
    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName()
    {
        CountryAddRequest countryAddRequest1 = new CountryAddRequest(){CountryName = "CANADA1"};
        CountryAddRequest countryAddRequest2 = new CountryAddRequest(){CountryName = "CANADA2"};
        CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
        CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);
        
        PersonAddRequest personAddRequest1 = new PersonAddRequest()
        {
            PersonName = "alex",
            PersonEmail = "asd@mail.ru",
            Address = "aasd my address is",
            CountryId = countryResponse1.CountryId,
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Parse("2000-01-01")
        };
        PersonAddRequest personAddRequest2 = new PersonAddRequest()
        {
            PersonName = "nick",
            PersonEmail = "asd@mail.ru",
            Address = "aasd my address is",
            CountryId = countryResponse2.CountryId,
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Parse("2000-01-01")
        };
        
        List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
        {
            personAddRequest1,
            personAddRequest2
        };
        List<PersonResponse> personResponses = new List<PersonResponse>();

        foreach (var variablePersonAddRequest in personAddRequests)
        {
            PersonResponse personResponse = await _personService.AddPerson(variablePersonAddRequest);
            personResponses.Add(personResponse);
        }
        
        _testOutputHelper.WriteLine("Expected:");
        foreach (var variablePersonResponse in personResponses)
        {
            _testOutputHelper.WriteLine(variablePersonResponse.ToString());
        }

        List<PersonResponse> personResponsesFromSearch = 
            await _personService.GetFilteredPersons(nameof(Person.PersonName), "nick");
        
        _testOutputHelper.WriteLine("Actual:");
        foreach (var variablePersonResponse in personResponsesFromSearch)
        {
            _testOutputHelper.WriteLine(variablePersonResponse.ToString());
        }

        foreach (var personResponse in personResponses)
        {
            if (personResponse.PersonName != null)
            {
                if (personResponse.PersonName.Contains("nick", StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Contains(personResponse, personResponsesFromSearch);
                }
            }
        }
    }

    #endregion
}