using Entities;
using ServiceContracts.Enums;
using ServiceContracts.DTO;

namespace ServiceContracts;

public interface IPersonAdderService
{
    Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);
}