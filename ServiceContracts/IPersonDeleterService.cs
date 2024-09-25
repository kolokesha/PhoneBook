using Entities;
using ServiceContracts.Enums;
using ServiceContracts.DTO;

namespace ServiceContracts;

public interface IPersonDeleterService
{
    Task<bool> DeletePerson(Guid? personId);
}