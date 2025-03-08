using Exptour.Application;
using Exptour.Application.Abstract.Services;
using Exptour.Application.Attributes;
using Exptour.Application.DTOs.User;
using Exptour.Common.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exptour.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("[Action]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<EmptyResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult> UpdatePassword(UpdatePasswordDTO updatePasswordDTO)
    {
        var response = await _userService.UpdatePasswordAsync(updatePasswordDTO);
        return response.ToActionResult();
    }

    [HttpGet("[Action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Get All Users", Menu = "Users")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<Pagination<UserResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAllUsers(int pageNumber, int take, bool isPaginated)
    {
        var response = await _userService.GetAllUsersAsync(pageNumber, take, isPaginated);
        return response.ToActionResult();
    }

    [HttpGet("[Action]/{userIdOrName}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Get Roles According To Users", Menu = "Users")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<List<string>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetRolesAccordingToUser(string userIdOrName)
    {
        var response = await _userService.GetRolesAccordingToUserAsync(userIdOrName);
        return response.ToActionResult();
    }

    [HttpPost("[Action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Assign Role To User", Menu = "Users")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<EmptyResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult> AssignRoleToUser(AssignRoleDTO assignRoleDTO)
    {
        var response = await _userService.AssignRoleToUserAsnyc(assignRoleDTO);
        return response.ToActionResult();
    }

    [HttpDelete("[Action]/{userId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Remove Role From User", Menu = "Users")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<EmptyResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult> RemoveRoleFromUser(string userId, [FromBody] string[] roles)
    {
        var response = await _userService.RemoveRoleFromUserAsync(userId, roles);
        return response.ToActionResult();
    }
}
