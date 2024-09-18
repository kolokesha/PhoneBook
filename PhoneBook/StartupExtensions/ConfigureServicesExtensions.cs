using Entities;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using PhoneBook.Filters.ActionFilters;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace PhoneBook.StartupExtensions;

public static class ConfigureServicesExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews();
        services.AddTransient<ResponseHeaderActionFilter>();

        services.AddControllersWithViews(options =>
        {
            var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
            options.Filters.Add(new ResponseHeaderActionFilter(logger)
            {
                Key = "My-Key-From-Global",
                Value = "My-Value_From-Global",
                Order = 2
            });
        });

        
        services.AddScoped<ICountriesRepository, CountriesRepository>();
        services.AddScoped<IPersonsRepository, PersonsRepository>();
        services.AddScoped<ICountriesService, CountriesService>();
        services.AddScoped<IPersonService, PersonsService>();
        services.AddTransient<PersonsListActionFilter>();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddHttpLogging(option =>
        {
            option.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
        });

        return services;
    }
}