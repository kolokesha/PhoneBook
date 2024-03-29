﻿using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{
    private readonly PhoneBookDbContext _db;

    public CountriesService(PhoneBookDbContext dbContext)
    {
        _db = dbContext;
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

        if (await _db.Countries.CountAsync(temp => temp.CountryName == countryAddRequest.CountryName) > 0)
        {
            throw new ArgumentException("Country name already exists");
        }

        Country country = countryAddRequest.ToCountry();

        country.CountryId = Guid.NewGuid();

        _db.Countries.Add(country);
        await _db.SaveChangesAsync();

        return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        return await _db.Countries.
            Select(country => country.ToCountryResponse()).ToListAsync();
        
    }

    public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
    {
        if (countryId == null)
        {
            return null;
        }

        Country? country_response_from_list = await _db.Countries.
            FirstOrDefaultAsync(temp => temp.CountryId == countryId);
        if (country_response_from_list == null)
        {
            return null;
        }
        return country_response_from_list.ToCountryResponse();
    }
}