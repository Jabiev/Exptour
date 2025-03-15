using Exptour.Application;
using Exptour.Application.Abstract.Services;
using Exptour.Application.Attributes;
using Exptour.Application.DTOs.City;
using Exptour.Common.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exptour.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CitiesController : ControllerBase
{
    private readonly ICityService _cityService;

    public CitiesController(ICityService cityService)
    {
        _cityService = cityService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<Pagination<CityResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll([FromQuery] int pageNumber,
        [FromQuery] int take,
        [FromQuery] bool isPaginated)
    {
        var response = await _cityService.GetAllAsync(pageNumber, take, isPaginated);
        return response.ToActionResult();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<CityResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetById(string id)
    {
        var response = await _cityService.GetByIdAsync(id);
        return response.ToActionResult();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Create City", Menu = "Cities")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<CityResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Create([FromBody] CityDTO request)
    {
        var response = await _cityService.CreateAsync(request);
        return response.ToActionResult();
    }

    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Update City", Menu = "Cities")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<CityResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Update(string id, [FromBody] UpdateCityDTO request)
    {
        var response = await _cityService.UpdateAsync(id, request);
        return response.ToActionResult();
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Delete City", Menu = "Cities")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<CityResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Delete(string id)
    {
        var response = await _cityService.DeleteAsync(id);
        return response.ToActionResult();
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<Pagination<CityResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Search([FromQuery] string clue,
        [FromQuery] int pageNumber,
        [FromQuery] int take,
        [FromQuery] bool isPaginated)
    {
        var response = await _cityService.GetByNameAsync(clue, pageNumber, take, isPaginated);
        return response.ToActionResult();
    }

    [HttpGet("search/{countryId}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<Pagination<CityResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> SearchByCountryId(string countryId,
        [FromQuery] int pageNumber,
        [FromQuery] int take,
        [FromQuery] bool isPaginated)
    {
        var response = await _cityService.GetByCountryIdAsync(countryId, pageNumber, take, isPaginated);
        return response.ToActionResult();
    }
}
