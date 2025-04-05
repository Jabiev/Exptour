using Exptour.Application;
using Exptour.Application.Abstract.Services;
using Exptour.Application.Attributes;
using Exptour.Application.DTOs.Guide;
using Exptour.Common.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exptour.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GuidesController : ControllerBase
{
    private readonly IGuideService _guideService;

    public GuidesController(IGuideService guideService)
    {
        _guideService = guideService;
    }

    [HttpPost("[Action]/{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminsOrTourismManagers")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Assign Schedule", Menu = "Guides")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<SetScheduleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> AssignSchedule(string id,
        [FromBody] AssignScheduleRequest request)
    {
        var response = await _guideService.AssignScheduleAsync(id, request.StartDate, request.EndDate);
        return response.ToActionResult();
    }

    [HttpPost("[Action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Assign Language", Menu = "Guides")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<AssignLanguageResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> AssignLanguage([FromBody] List<string> languageIds)
    {
        var response = await _guideService.AssignLanguageAsync(languageIds);
        return response.ToActionResult();
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminsOrTourismManagers")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Create Guide", Menu = "Guides")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<CreateGuideResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Create([FromBody] CreateGuideDTO request)
    {
        var response = await _guideService.CreateAsync(request);
        return response.ToActionResult();
    }

    [HttpGet("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<GuideResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetById(string id)
    {
        var response = await _guideService.GetByIdAsync(id);
        return response.ToActionResult();
    }

    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<GuideStatisticsResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetStatistics()
    {
        var response = await _guideService.GetGuidesStatisticsAsync();
        return response.ToActionResult();
    }

    [HttpGet("[Action]")]//LOOK At
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Guides")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<Pagination<GetGuideSchedulesResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetGuideSchedules([FromQuery] int pageNumber,
        [FromQuery] int take,
        [FromQuery] bool isPaginated)
    {
        var response = await _guideService.GetGuideSchedulesAsync(pageNumber, take, isPaginated);
        return response.ToActionResult();
    }

    [HttpGet("ByLanguage")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<Pagination<GetAllByLanguageResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetGuidesByLanguage([FromQuery] string clue,
        [FromQuery] int pageNumber,
        [FromQuery] int take,
        [FromQuery] bool isPaginated)
    {
        var response = await _guideService.GetAllByLanguageAsync(clue, pageNumber, take, isPaginated);
        return response.ToActionResult();
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminsOrTourismManagers")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<Pagination<GetAllResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll([FromQuery] int pageNumber,
        [FromQuery] int take,
        [FromQuery] bool isPaginated)
    {
        var response = await _guideService.GetAllAsync(pageNumber, take, isPaginated);
        return response.ToActionResult();
    }

    [HttpPost("[Action]/{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminsOrTourismManagers")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Set On Leave", Menu = "Guides")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<GuideResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> SetOnLeave(string id,
        [FromQuery] int day)
    {
        var response = await _guideService.SetOnLeaveAsync(id, day);
        return response.ToActionResult();
    }

    [HttpPost("[Action]/{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminsOrTourismManagers")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Set Retire", Menu = "Guides")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<GuideResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> SetRetire(string id)
    {
        var response = await _guideService.SetRetireAsync(id);
        return response.ToActionResult();
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Delete Guide", Menu = "Guides")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<EmptyResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Delete(string id)
    {
        var response = await _guideService.DeleteAsync(id);
        return response.ToActionResult();
    }

    [HttpPut("[Action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Update Guide", Menu = "Guides")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<GuideResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Update([FromBody] UpdateGuideDTO request)
    {
        var response = await _guideService.UpdateAsync(request);
        return response.ToActionResult();
    }

    [HttpPut("[Action]/{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminsOrTourismManagers")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Update Guide Schedule", Menu = "Guides")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<SetScheduleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> UpdateSchedule(string id,
        [FromBody] UpdateGuideScheduleRequest request)
    {
        var response = await _guideService.UpdateScheduleAsync(id, request.ScheduleId, request.NewStartDate, request.NewEndDate);
        return response.ToActionResult();
    }

    [HttpPost("[Action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Unassign Language", Menu = "Guides")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<GuideResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> UnassignLanguage([FromBody] List<string> languageIds)
    {
        var response = await _guideService.UnassignLanguageAsync(languageIds);
        return response.ToActionResult();
    }
}
