using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PhoneBook.Filters.ActionFilters;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace PhoneBook.Controllers;

[Route("[controller]")]
[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object []
{
    "X-Custom-Key", "Custom-Value", 3
}, Order = 3)]
public class PersonsController : Controller
{
    private readonly IPersonService _personsService;
    private readonly ICountriesService _countriesService;
    private readonly ILogger<PersonsController> _logger;

    public PersonsController(IPersonService personsService, ICountriesService countriesService, ILogger<PersonsController> logger)
    {
        _personsService = personsService;
        _countriesService = countriesService;
        _logger = logger;
    }

    [Route("[action]")]
    [Route("/")]
    [TypeFilter(typeof(PersonsListActionFilter), Order = 4)]
    [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object []
    {
        "X-Custom-Key", "Custom-Value", 1 
    }, Order = 1)]
    public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName),
        SortOrderOptions sortOrder = SortOrderOptions.Asc)
    {
        _logger.LogInformation("Index action methond of PersonsControlle");

        _logger.LogDebug($"searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}," +
                         $" sortOrder: {sortOrder}");
        
        List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);

        List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(persons, sortBy, sortOrder);

        return View(sortedPersons);
    }

    [Route("[action]")]
    [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object []
    {
        "X-Custom-Key2", "Custom-Value2"
    })]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        List<CountryResponse> countries = await _countriesService.GetAllCountries();
        ViewBag.Countries = countries.Select(temp => new SelectListItem()
        {
            Text = temp.CountryName,
            Value = temp.CountryId.ToString()
        });
        return View();
    }

    [Route("[action]")]
    [HttpPost]
    public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
    {
        if (!ModelState.IsValid)
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
                new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });
            ViewBag.Errors = ModelState.Values.SelectMany
                (v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View();
        }

        PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);
        return RedirectToAction("Index", "Persons");
    }

    [HttpGet]
    [Route("[action]/{personId:guid}")]
    public async Task<IActionResult> Edit(Guid personId)
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonId(personId);
        if (personResponse == null)
        {
            return RedirectToAction("Index");
        }

        PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
        List<CountryResponse> countries = await _countriesService.GetAllCountries();
        ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });
        ViewBag.Errors = ModelState.Values.SelectMany(
            v => v.Errors).Select(e => e.ErrorMessage).ToList();
        

        return View(personUpdateRequest);
    }

    [HttpPost]
    [Route("[action]/{personId:guid}")]
    public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest, Guid personId)
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonId(personId);
        if (personResponse == null)
        {
            return RedirectToAction("Index");
        }

        if (ModelState.IsValid)
        {
            await _personsService.UpdatePerson(personUpdateRequest);
            return RedirectToAction("Index");
        }

        List<CountryResponse> countries = await _countriesService.GetAllCountries();
        ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });
        ViewBag.Errors = ModelState.Values.SelectMany
            (v => v.Errors).Select(e => e.ErrorMessage).ToList();
        return View(personResponse.ToPersonUpdateRequest());
    }

    [HttpGet]
    [Route("[action]/{personId:guid}")]
    public async Task<IActionResult> Delete(Guid personId)
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonId(personId);
        if (personResponse == null)
        {
            return RedirectToAction("Index");
        }

        return View(personResponse);
    }
    
    [HttpPost]
    [Route("[action]/{personId:guid}")]
    public async Task<IActionResult> Delete(PersonUpdateRequest person, Guid personId)
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonId(personId);
        if (personResponse == null)
        {
            return RedirectToAction("Index");
        }

        await _personsService.DeletePerson(personId);
        return RedirectToAction("Index");
    }


    //TO DO: Refactor this for mac os or mb another tool for pdf
    // [Route("PersonsPDF")]
    // public async Task<IActionResult> PersonsPDF()
    // {
    //     List<PersonResponse> persons = await _personsService.GetAllPersons();
    //     return new ViewAsPdf("PersonsPDF", persons, ViewData)
    //     {
    //         PageMargins = new Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20},
    //         PageOrientation = Orientation.Landscape
    //     };
    // }
    
}