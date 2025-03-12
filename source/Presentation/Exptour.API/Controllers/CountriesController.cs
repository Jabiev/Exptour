using Exptour.Application;
using Exptour.Application.Abstract.Services;
using Exptour.Application.Attributes;
using Exptour.Application.DTOs.Country;
using Exptour.Common.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exptour.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly ICountryService _countryService;

    public CountriesController(ICountryService countryService)
    {
        _countryService = countryService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<Pagination<CountryResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(int pageNumber, int take, bool isPaginated)
    {
        var response = await _countryService.GetAllAsync(pageNumber, take, isPaginated);
        return response.ToActionResult();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<CountryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(string id)
    {
        var response = await _countryService.GetByIdAsync(id);
        return response.ToActionResult();
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Delete Country", Menu = "Countries")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<CountryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(string id)
    {
        var response = await _countryService.DeleteAsync(id);
        return response.ToActionResult();
    }

    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Update Country", Menu = "Countries")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<CountryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromRoute] string id, CountryDTO countryDTO)
    {
        var response = await _countryService.UpdateAsync(id, countryDTO);
        return response.ToActionResult();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Create Country", Menu = "Countries")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<CountryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(CountryDTO countryDTO)
    {
        var response = await _countryService.CreateAsync(countryDTO);
        return response.ToActionResult();
    }

    [HttpGet("search/{name}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<CountryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByName(string name)
    {
        var response = await _countryService.GetByNameAsync(name);
        return response.ToActionResult();
    }
}
