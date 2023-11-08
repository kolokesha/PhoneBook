using Microsoft.EntityFrameworkCore;

namespace Entities;

public class PhoneBookDbContext : DbContext
{
    public PhoneBookDbContext(DbContextOptions<PhoneBookDbContext> options) : base (options)
    {
        
    }
    
    public DbSet<Person> Persons { get; set; }
    public DbSet<Country> Countries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Country>().ToTable("Countries");
        modelBuilder.Entity<Person>().ToTable("Persons");
        modelBuilder.Entity<Gender>().ToTable("Genders");

        string countriesJson = File.ReadAllText("countries.json");
        List<Country>? countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);

        if (countries != null)
            foreach (var country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

        string personsJson = File.ReadAllText("persons.json");
        List<Person>? persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);

        if (persons != null)
            foreach (var person in persons)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }
        
        string gendersJson = File.ReadAllText("genders.json");
        List<Gender>? genders = System.Text.Json.JsonSerializer.Deserialize<List<Gender>>(gendersJson);

        if (genders != null)
            foreach (var gender in genders)
            {
                modelBuilder.Entity<Gender>().HasData(gender);
            }

        modelBuilder.Entity<Person>().Property(temp => temp.TIN)
            .HasColumnName("TaxIdentificationNumber")
            .HasColumnType("varchar(8)")
            .HasDefaultValue("ABC123");

        // modelBuilder.Entity<Person>()
        //     .HasIndex(temp => temp.TIN).IsUnique();
        
    }
}