global using Xunit;
using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
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
    private readonly IFixture _fixture;

    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture();
        var countriesInitialData = new List<Country>();
        var persosnInitialData = new List<Person>();
        DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

        ApplicationDbContext dbContext = dbContextMock.Object;
        dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
        dbContextMock.CreateDbSetMock(temp => temp.Persons, persosnInitialData);
        
        _countriesService = new CountriesService(null);
        _personService = new PersonsService(null);
        
        _testOutputHelper = testOutputHelper;
    }

    #region AddPerson
    
    [Fact]
    public async Task AddPerson_NullPerson()
    {
        PersonAddRequest personAddRequest = null;
        Func<Task> action = async () => await _personService.AddPerson(personAddRequest);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }
    
    [Fact]
    public async Task AddPerson_PersonNameNull()
    {
        PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, null as string)
            .Create();
        
        await Assert.ThrowsAsync<ArgumentException>(async () => await _personService.AddPerson(personAddRequest));
    }
    
    [Fact]
    public async Task AddPerson_ProperPersonDetails()
    {
        var personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp=> temp.PersonEmail, "someone@example.com")
            .Create();

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
        CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
        CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

        PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonEmail, "email@sample.com")
            .Create();
        
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
        CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();
        CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
        CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);
        
        PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonEmail, "email@sample.com")
            .Create();
        PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonEmail, "email@sample2.com")
            .Create();
        
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
        CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();
        CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
        CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);
        
        PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonEmail, "email@sample.com")
            .Create();
        PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonEmail, "email@sample.com")
            .Create();
        
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
        CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();
        CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
        CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);
        
        PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonEmail, "email@sample.com")
            .Create();
        PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonEmail, "email@sample.com")
            .Create();
        
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