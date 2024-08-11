using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{
    private readonly ICountriesRepository _countriesRepository;

    public CountriesService(ICountriesRepository countriesRepository)
    {
        _countriesRepository = countriesRepository;
    }

    public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
    {
        if (countryAddRequest is null)
        {
            throw new ArgumentNullException(nameof(countryAddRequest));
        }

        if (countryAddRequest.CountryName is null)
        {
            throw new ArgumentException(nameof(countryAddRequest));
        }

        if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
        {
            throw new ArgumentException("Country name already exists");
        }

        Country country = countryAddRequest.ToCountry();

        country.CountryId = Guid.NewGuid();

        await _countriesRepository.AddCountry(country);

        return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        return (await _countriesRepository.GetAllCountries()).
            Select(country => country.ToCountryResponse()).ToList();
        
    }

    public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
    {
        if (countryId == null)
        {
            return null;
        }

        Country? country_response_from_list = await _countriesRepository.GetCountryByCountryId(countryId.Value);
            
        if (country_response_from_list == null)
        {
            return null;
        }
        return country_response_from_list.ToCountryResponse();
    }
}