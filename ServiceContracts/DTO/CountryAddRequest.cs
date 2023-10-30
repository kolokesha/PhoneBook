using Entities;

namespace ServiceContracts.DTO;

public class CountryAddRequest
{
    /// <summary>
    /// DTO class for adding new country
    /// </summary>
    public string? CountryName { get; set; }

    public Country ToCountry()
    {
        return new Country()
        {
            CountryName = CountryName
        };
    }
}