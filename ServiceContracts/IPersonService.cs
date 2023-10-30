using Entities;
using ServiceContracts.Enums;
using ServiceContracts.DTO;

namespace ServiceContracts;

public interface IPersonService
{
    Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

    Task<List<PersonResponse>> GetAllPersons();

    Task<PersonResponse?> GetPersonByPersonId(Guid? personId);

    /// <summary>
    /// Return list of persons that matches with given search field and search string
    /// </summary>
    /// <param name="searchBy">search field</param>
    /// <param name="searchString">search string</param>
    /// <returns></returns>
    Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

    /// <summary>
    /// Return sorted list of persons asc or desc
    /// </summary>
    /// <param name="allPersons"></param>
    /// <param name="sortBy"></param>
    /// <param name="sortOrder"></param>
    /// <returns></returns>
    Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);

    Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
    Task<bool> DeletePerson(Guid? personId);
}