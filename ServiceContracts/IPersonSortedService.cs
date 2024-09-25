using Entities;
using ServiceContracts.Enums;
using ServiceContracts.DTO;

namespace ServiceContracts;

public interface IPersonSortedService
{
    /// <summary>
    /// Return sorted list of persons asc or desc
    /// </summary>
    /// <param name="allPersons"></param>
    /// <param name="sortBy"></param>
    /// <param name="sortOrder"></param>
    /// <returns></returns>
    Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);
}