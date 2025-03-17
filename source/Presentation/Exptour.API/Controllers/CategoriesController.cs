using Exptour.Application;
using Exptour.Application.Abstract.Services;
using Exptour.Application.Attributes;
using Exptour.Application.DTOs.Category;
using Exptour.Common.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exptour.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<Pagination<CategoryResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll([FromQuery] int pageNumber,
        [FromQuery] int take,
        [FromQuery] bool isPaginated)
    {
        var response = await _categoryService.GetAllAsync(pageNumber, take, isPaginated);
        return response.ToActionResult();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Create Category", Menu = "Categories")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Create([FromBody] CategoryDTO request)
    {
        var response = await _categoryService.CreateAsync(request);
        return response.ToActionResult();
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Delete Category", Menu = "Categories")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<EmptyResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Delete(string id)
    {
        var response = await _categoryService.DeleteAsync(id);
        return response.ToActionResult();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetById(string id)
    {
        var response = await _categoryService.GetByIdAsync(id);
        return response.ToActionResult();
    }   
}
