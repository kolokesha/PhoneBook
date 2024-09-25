using Entities;
using ServiceContracts.Enums;
using ServiceContracts.DTO;

namespace ServiceContracts;

public interface IPersonUpdaterService
{
    Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
}