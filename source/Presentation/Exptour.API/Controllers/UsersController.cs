using Exptour.Application;
using Exptour.Application.Abstract.Services;
using Exptour.Application.Attributes;
using Exptour.Application.DTOs.User;
using Exptour.Common.Shared;
using Exptour.Infrastructure.ElasticSearch.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exptour.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserSearchService _userSearchService;
    public UsersController(IUserService userService,
        IUserSearchService userSearchService)
    {
        _userService = userService;
        _userSearchService = userSearchService;
    }

    [HttpPost("[Action]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<RegisterResponse?>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Register([FromBody] RegisterDTO request)
    {
        var response = await _userService.RegisterAsync(request);
        return response.ToActionResult();
    }

    [HttpPost("[Action]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<EmptyResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult> ResetPasswordRequest([FromBody] PasswordResetDTO request)
    {
        var response = await _userService.PasswordResetAsnyc(request);
        return response.ToActionResult();
    }

    [HttpPost("[Action]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult> VerifyResetToken([FromBody] VerifyResetTokenDto request)
    {
        var response = await _userService.VerifyResetTokenAsync(request);
        return response.ToActionResult();
    }

    [HttpPost("[Action]/{id}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<EmptyResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult> UpdatePassword(string id,
        [FromBody] UpdatePasswordDTO request)
    {
        var response = await _userService.UpdatePasswordAsync(id, request);
        return response.ToActionResult();
    }

    [HttpGet("[Action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Get All Users", Menu = "Users")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<Pagination<UserResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll([FromQuery] int pageNumber,
        [FromQuery] int take,
        [FromQuery] bool isPaginated)
    {
        var response = await _userService.GetAllUsersAsync(pageNumber, take, isPaginated);
        return response.ToActionResult();
    }

    [HttpGet("Search")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<List<UserResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> SearchUsers([FromQuery] string name,
        [FromQuery] int pageNumber,
        [FromQuery] int take,
        [FromQuery] bool isPaginated)
    {
        var response = await _userService.SearchAsync(name, pageNumber, take, isPaginated);
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
    public async Task<ActionResult> AssignRoleToUser([FromBody] AssignRoleDTO request)
    {
        var response = await _userService.AssignRoleToUserAsnyc(request);
        return response.ToActionResult();
    }

    [HttpPut("[Action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<UpdateProfileResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> UpdateProfile([FromBody] UpdateProfileDTO request)
    {
        var response = await _userService.UpdateProfileAsync(request);
        return response.ToActionResult();
    }

    [HttpGet("[Action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Get Permissions According To User", Menu = "Users")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<List<bool>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> HasRolePermissionToEndpoint([FromQuery] string userName,
        [FromQuery] string code)
    {
        var response = await _userService.HasRolePermissionToEndpointAsync(userName, code);
        return response.ToActionResult();
    }

    [HttpPost("[Action]/{userId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "OnlyAdmins")]
    [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Remove Role From User", Menu = "Users")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<EmptyResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult> RemoveRoleFromUser(string userId,
        [FromBody] string[] roles)
    {
        var response = await _userService.RemoveRoleFromUserAsync(userId, roles);
        return response.ToActionResult();
    }
}
