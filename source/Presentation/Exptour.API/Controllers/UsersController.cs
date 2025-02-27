using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.User;
using Exptour.Common.Shared;
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
}
