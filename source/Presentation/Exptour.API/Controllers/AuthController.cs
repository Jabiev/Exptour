using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.Auth;
using Exptour.Common.Shared;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Exptour.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IGoogleService _googleService;
    public AuthController(IAuthService authService, IGoogleService googleService)
    {
        _authService = authService;
        _googleService = googleService;
    }

    [HttpPost("Connect/[Action]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<TokenResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Login(SignInDTO signInDTO)
    {
        var response = await _authService.Login(signInDTO);
        return response.ToActionResult();
    }

    [HttpPost("Connect/ValidateGoogleToken")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<GoogleJsonWebSignature.Payload>), StatusCodes.Status200OK)]
    public async Task<ActionResult> ValidateGoogleToken([FromBody] string idToken)
    {
        var response = await _googleService.ValidateGoogleTokenAsync(idToken);
        return response.ToActionResult();
    }

    [HttpPost("Connect/Token")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<TokenResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> RefreshToken(string refreshToken)
    {
        var response = await _authService.RefreshToken(refreshToken);
        return response.ToActionResult();
    }

    [HttpPost("[Action]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(APIResponse<object?>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Register(RegisterDTO registerDTO)
    {
        var response = await _authService.Register(registerDTO);
        return response.ToActionResult();
    }

}
