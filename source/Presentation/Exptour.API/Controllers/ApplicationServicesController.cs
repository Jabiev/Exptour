using Exptour.Application;
using Exptour.Application.Abstract.Services;
using Exptour.Application.Attributes;
using Exptour.Common.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exptour.API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
[Route("api/[controller]")]
[ApiController]
public class ApplicationServicesController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationServicesController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpGet("[Action]")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Get Authorize Definition Endpoints", Menu = "Application Services")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<List<Application.DTOs.Menu>>), StatusCodes.Status200OK)]
    public ActionResult GetAuthorizeDefinitionEndpoints()
    {
        var response = _applicationService.GetAuthorizeDefinitionEndpoints(typeof(Program));
        return response.ToActionResult();
    }
}
