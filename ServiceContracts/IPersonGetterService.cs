using Entities;
using ServiceContracts.Enums;
using ServiceContracts.DTO;

namespace ServiceContracts;

public interface IPersonGetterService
{

    Task<List<PersonResponse>> GetAllPersons();

    Task<PersonResponse?> GetPersonByPersonId(Guid? personId);

    /// <summary>
    /// Return list of persons that matches with given search field and search string
    /// </summary>
    /// <param name="searchBy">search field</param>
    /// <param name="searchString">search string</param>
    /// <returns></returns>
    Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);
}