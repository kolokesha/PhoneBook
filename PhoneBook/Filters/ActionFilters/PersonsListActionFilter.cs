using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc.Filters;
using PhoneBook.Controllers;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace PhoneBook.Filters.ActionFilters;

public class PersonsListActionFilter : IActionFilter
{
    private readonly ILogger<PersonsListActionFilter> _logger;

    public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        
        _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuted));
        context.HttpContext.Items["arguments"] = context.ActionArguments;
        if (context.ActionArguments.ContainsKey("searchBy"))
        {
            string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);
            
            if (!string.IsNullOrEmpty(searchBy))
            {
                var searchByOptions = new List<string>()
                {
                    nameof(PersonResponse.PersonName),
                    nameof(PersonResponse.PersonEmail),
                    nameof(PersonResponse.DateOfBirth),
                    nameof(PersonResponse.Gender),
                    nameof(PersonResponse.CountryId),
                    nameof(PersonResponse.Address), 
                };
                
                if (searchByOptions.Any(temp => temp == searchBy) == false)
                {
                    _logger.LogInformation($"searchBy actual value {searchBy}");
                    context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                    _logger.LogInformation($"searchBy update value {searchBy}");
                }
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter), nameof(OnActionExecuting));

        PersonsController personsController = (PersonsController)context.Controller;
        IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];
        
        if (parameters != null)
        {
            if (parameters.ContainsKey("CurrentSearchBy"))
            {
                personsController.ViewData["CurrentSearchBy"] = Convert.ToString(parameters["CurrentSearchBy"]);
            } 
            
            if (parameters.ContainsKey("CurrentSearchString"))
            {
                personsController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["CurrentSearchString"]);
            }
            
            if (parameters.ContainsKey("CurrentSortBy"))
            {
                personsController.ViewData["CurrentSortBy"] = Convert.ToString(parameters["CurrentSortBy"]);
            }
            else
            {
                personsController.ViewData["CurrentSortBy"] = nameof(PersonResponse.PersonName);
            }
            
            if (parameters.ContainsKey("CurrentSortOrder"))
            {
                personsController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["CurrentSortOrder"]);
            }
            else
            {
                personsController.ViewData["CurrentSortOrder"] = nameof(SortOrderOptions.Asc);
            }
        }
        personsController.ViewBag.SearchFields = new Dictionary<string, string>()
        {
            { nameof(PersonResponse.PersonName), "Person Name" },
            { nameof(PersonResponse.PersonEmail), "Email" },
            { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
            { nameof(PersonResponse.Gender), "Gender" },
            { nameof(PersonResponse.CountryId), "Country" },
            { nameof(PersonResponse.Address), "Address" },
            { nameof(PersonResponse.ReceiveNewsLetters), "Receive News Letters" },
        };
    }
}