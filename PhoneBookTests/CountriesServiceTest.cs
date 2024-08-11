using ServiceContracts;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.DTO;
using Services;

namespace PhoneBookTests;

public class CountriesServiceTest
{
    private readonly ICountriesService _countriesService;

    public CountriesServiceTest()
    {
        var countriesInitialData = new List<Country>();
        DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

        ApplicationDbContext dbContext = dbContextMock.Object;
        dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
        _countriesService = new CountriesService(null);
    }

    #region AddCountry

    [Fact]
    public async Task AddCountry_NullCountry()
    {
        CountryAddRequest request = null;

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _countriesService.AddCountry(request);
        });
    }
    
    [Fact]
    public async Task AddCountry_CountryNameIsNull()
    {
        CountryAddRequest request = new CountryAddRequest()
        {
            CountryName = null
        };

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _countriesService.AddCountry(request);
        });
    }
    
    [Fact]
    public async Task AddCountry_CountryNameIsDuplicate()
    {
        CountryAddRequest request1 = new CountryAddRequest()
        {
            CountryName = "Russia"
        };
        CountryAddRequest request2 = new CountryAddRequest()
        {
            CountryName = "Russia"
        };

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _countriesService.AddCountry(request1);
            await _countriesService.AddCountry(request1);
        });
    }
    
    [Fact]
    public async Task AddCountry_ProperCountryDetails()
    {
        CountryAddRequest request = new CountryAddRequest()
        {
            CountryName = "Russia"
        };
        
        CountryResponse response = await _countriesService.AddCountry(request);
        List<CountryResponse> countryResponses = await _countriesService.GetAllCountries();

        Assert.True(response.CountryId != Guid.Empty && response.CountryName != null);
        Assert.Contains(response, countryResponses);
    }
    
    #endregion

    #region GetAllCountries

    [Fact]
    public async Task GetAllCountries_EmptyList()
    {
        List<CountryResponse> actualCountryFromResponseList = await _countriesService.GetAllCountries();
        
        Assert.Empty(actualCountryFromResponseList);
    }

    [Fact]
    public async Task GetAllCountries_AddFewCountries()
    {
        List<CountryAddRequest> actualCountryFromResponseList = new List<CountryAddRequest>()
        {
            new CountryAddRequest() { CountryName = "Russia" },
            new CountryAddRequest() { CountryName = "Japan" },
            new CountryAddRequest() { CountryName = "Thailand" },
        };

        List<CountryResponse> countriesListFromAddCountry = new List<CountryResponse>();

        foreach (CountryAddRequest country_request in actualCountryFromResponseList)
        {
            countriesListFromAddCountry.Add(await _countriesService.AddCountry(country_request));
        }

        List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

        foreach (var countryResponse in actualCountryResponseList)
        {
            Assert.Contains(countryResponse, actualCountryResponseList);
        }
    }
    

    #endregion

    #region GetCountryByCountryId

    [Fact]
    //Tests null value
    public async Task GetCountryByCountryId_NullCountryId()
    {
        Guid? countryId = null;

        CountryResponse? country_response = await _countriesService.GetCountryByCountryId(countryId);
        Assert.Null(country_response);
    }

    [Fact]
    //Tests correct value
    public async Task GetCountryByCountryId_ValidCountryId()
    {
        CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "China" };
        CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

        CountryResponse? country_response_from_get =
            await _countriesService.GetCountryByCountryId(country_response_from_add.CountryId);
        
        Assert.Equal(country_response_from_add, country_response_from_get);
    }

    #endregion
    
}