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
    public async Task<ActionResult> GetAll([FromQuery] int pageNumber,
        [FromQuery] int take,
        [FromQuery] bool isPaginated)
    {
        var response = await _countryService.GetAllAsync(pageNumber, take, isPaginated);
        return response.ToActionResult();
    }

    [HttpGet("with-cities")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<Pagination<CountryWithCitiesResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAllWithCities([FromQuery] int pageNumber,
        [FromQuery] int take,
        [FromQuery] bool isPaginated)
    {
        var response = await _countryService.GetAllWithCitiesAsync(pageNumber, take, isPaginated);
        return response.ToActionResult();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<CountryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetById(string id)
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
    public async Task<ActionResult> Delete(string id)
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
    public async Task<ActionResult> Update(string id,
        [FromBody] CountryDTO request)
    {
        var response = await _countryService.UpdateAsync(id, request);
        return response.ToActionResult();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Create Country", Menu = "Countries")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<CountryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Create([FromBody] CountryDTO request)
    {
        var response = await _countryService.CreateAsync(request);
        return response.ToActionResult();
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<CountryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetByName([FromQuery] string name,
        [FromQuery] int pageNumber,
        [FromQuery] int take,
        [FromQuery] bool isPaginated)
    {
        var response = await _countryService.GetByNameAsync(name, pageNumber, take, isPaginated);
        return response.ToActionResult();
    }
}
