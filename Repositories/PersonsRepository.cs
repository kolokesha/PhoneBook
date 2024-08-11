using System.Linq.Expressions;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;

namespace Repositories;

public class PersonsRepository : IPersonsRepository
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<PersonsRepository> _logger;

    public PersonsRepository(ApplicationDbContext db, ILogger<PersonsRepository> logger)
    {
        _db = db;
        _logger = logger;
    }
    
    
    public async Task<Person> AddPerson(Person person)
    {
        _db.Persons.Add(person);
        await _db.SaveChangesAsync();
        
        return person;
    }

    public async Task<List<Person>> GetAllPersons()
    {
        _logger.LogInformation("GetAllPersons of Person Repository");
        return await _db.Persons.Include("Country").ToListAsync();
    }

    public async Task<Person?> GetPersonByPersonId(Guid personId)
    {
        return await _db.Persons.Include("Country").FirstOrDefaultAsync(temp=> temp.PersonId == personId);
    }

    public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
    {
        _logger.LogInformation("GetFilteredPersons of Person Repository");
        return await _db.Persons.Include("Country").Where(predicate).ToListAsync();
    }

    public async Task<bool> DeletePersonByPersonId(Guid personId)
    {
        _db.Persons.RemoveRange(_db.Persons.Where(temp => temp.PersonId == personId));
        int rowsDeleted = await _db.SaveChangesAsync();

        return rowsDeleted > 0;
    }

    public async Task<Person> UpdatePerson(Person person)
    {
        Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == person.PersonId);

        if (matchingPerson == null)
        {
            return person;
        }

        matchingPerson.PersonName = person.PersonName;
        matchingPerson.PersonEmail = person.PersonEmail;
        matchingPerson.DateOfBirth = person.DateOfBirth;
        matchingPerson.GenderId = person.GenderId;
        matchingPerson.CountryId = person.CountryId;
        matchingPerson.Address = person.Address;
        matchingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;
        matchingPerson.TIN = person.TIN;

        int countUpdatedRows = await _db.SaveChangesAsync();

        return matchingPerson;
    }
}