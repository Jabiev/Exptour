using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.AuthEndpoints;
using Exptour.Common.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exptour.API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
[Route("api/[controller]")]
[ApiController]
public class AuthorizationEndpointsController : ControllerBase
{
    private readonly IAuthorizationEndpointService _authorizationEndpointService;

    public AuthorizationEndpointsController(IAuthorizationEndpointService authorizationEndpointService)
    {
        _authorizationEndpointService = authorizationEndpointService;
    }

    [HttpPost("[Action]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<EmptyResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {
        var response = await _authorizationEndpointService.AssignRoleEndpointAsync(request.Roles, request.Menu, request.Code, typeof(Program));
        return response.ToActionResult();
    }

    [HttpGet("[Action]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<List<string>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetRolesToEndpoint([FromQuery] string code,
        [FromQuery] string menu)
    {
        var response = await _authorizationEndpointService.GetRolesAccordingToEndpointAsync(code, menu);
        return response.ToActionResult();
    }
}
