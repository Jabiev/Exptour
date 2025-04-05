using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.Auth;
using Exptour.Application.DTOs.Google;
using Exptour.Common.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Exptour.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IGoogleService _googleService;

    public AuthController(IAuthService authService,
        IGoogleService googleService)
    {
        _authService = authService;
        _googleService = googleService;
    }

    [HttpPost("Connect/[Action]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<TokenResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Login([FromBody] SignInDTO request)
    {
        var response = await _authService.Login(request);
        return response.ToActionResult();
    }

    [HttpPost("Connect/[Action]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<GoogleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GoogleLogin([FromBody] string idToken)
    {
        var response = await _googleService.GoogleLoginAsync(idToken);
        return response.ToActionResult();
    }

    [HttpPost("Connect/Token")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<TokenResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> RefreshToken([FromQuery] string refreshToken)
    {
        var response = await _authService.RefreshToken(refreshToken);
        return response.ToActionResult();
    }
}
