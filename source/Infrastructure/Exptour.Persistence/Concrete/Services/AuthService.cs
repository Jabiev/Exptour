using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.Auth;
using Exptour.Application.DTOs.Mail;
using Exptour.Application.Validators.Auth;
using Exptour.Application.Validators.Mail;
using Exptour.Common.Helpers;
using Exptour.Common.Infrastructure.Services;
using Exptour.Common.Shared;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Exptour.Persistence.Concrete.Services;

public class AuthService : BaseService, IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJWTService _jWTService;
    private readonly IMailService _mailService;
    private readonly IConfiguration _configuration;
    private readonly TourismManagementDbContext _tourismManagementDbContext;

    public AuthService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJWTService jWTService,
        IMailService mailService,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        TourismManagementDbContext tourismManagementDbContext) : base(httpContextAccessor, configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jWTService = jWTService;
        _mailService = mailService;
        _configuration = configuration;
        _tourismManagementDbContext = tourismManagementDbContext;
    }

    public async Task<APIResponse<TokenResponse>> Login(SignInDTO signInDTO)
    {
        var response = new APIResponse<TokenResponse>();

        SignInDTOValidator validations = new();

        var result = await validations.ValidateAsync(signInDTO);

        if (!result.IsValid)
        {
            StringBuilder stringBuilder = new();

            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.ErrorMessage);

            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization("InvalidRequest").state;
            return response;
        }

        var appUser = await _userManager.FindByEmailAsync(signInDTO.UserNameOrEmail);
        if (appUser is null)
        {
            appUser = await _userManager.FindByNameAsync(signInDTO.UserNameOrEmail);
            if (appUser is null)
            {
                var msgInvalidLogin = GetMessageByLocalization("InvalidLogin");
                response.Message = msgInvalidLogin.message;
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.State = msgInvalidLogin.state;
                return response;
            }
        }

        SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(appUser, signInDTO.Password, true);

        if (!signInResult.Succeeded)
        {
            var msgWrongPassword = GetMessageByLocalization("WrongPassword");
            response.Message = msgWrongPassword.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgWrongPassword.state;
            return response;
        }
        if (!appUser.IsActive)
        {
            var msgUserNotActive = GetMessageByLocalization("UserDoesNotActive");
            response.Message = msgUserNotActive.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgUserNotActive.state;
            return response;
        }

        List<Claim> claims = new()
        {
            new(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
            new(ClaimTypes.Name, appUser.UserName),
            new(ClaimTypes.GivenName, appUser.FullName)
        };

        foreach (var role in await _userManager.GetRolesAsync(appUser))
            claims.Add(new(ClaimTypes.Role, role));

        string accessToken = _jWTService.GenerateAccessToken(claims);
        string refreshToken = _jWTService.GenerateRefreshToken();
        _ = int.TryParse(_configuration["JWTSettings:RefreshTokenExpirationMinutes"], out int refreshTokenExpiryTime);
        appUser.RefreshToken = refreshToken;
        appUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenExpiryTime);
        await _userManager.UpdateAsync(appUser);

        response.Payload = new TokenResponse(accessToken, refreshToken, appUser.RefreshTokenExpiryTime.ToUAE());
        response.ResponseCode = HttpStatusCode.OK;
        return response;
    }

    public async Task<APIResponse<TokenResponse>> RefreshToken(string requestRefreshToken)
    {
        var response = new APIResponse<TokenResponse>();

        var appUser = await _tourismManagementDbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == requestRefreshToken);

        if (appUser is null)
        {
            var msgInvalidRefreshToken = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRefreshToken.message;
            response.State = msgInvalidRefreshToken.state;
            return response;
        }

        if (appUser.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            var msgRefreshTokenExpired = GetMessageByLocalization("RefreshTokenExpired");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgRefreshTokenExpired.message;
            response.State = msgRefreshTokenExpired.state;
            return response;
        }

        List<Claim> claims = new()
        {
            new(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
            new(ClaimTypes.Name, appUser.UserName),
            new(ClaimTypes.GivenName, appUser.FullName)
        };

        foreach (var role in await _userManager.GetRolesAsync(appUser))
            claims.Add(new(ClaimTypes.Role, role));

        string accessToken = _jWTService.GenerateAccessToken(claims);
        string refreshToken = _jWTService.GenerateRefreshToken();
        _ = int.TryParse(_configuration["JWTSettings:RefreshTokenExpirationMinutes"], out int refreshTokenExpiryTime);
        appUser.RefreshToken = refreshToken;
        appUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenExpiryTime);
        await _userManager.UpdateAsync(appUser);

        response.Payload = new TokenResponse(accessToken, refreshToken, appUser.RefreshTokenExpiryTime.ToUAE());
        response.ResponseCode = HttpStatusCode.OK;
        return response;
    }

    public async Task<APIResponse<object?>> Register(RegisterDTO registerDTO)
    {
        var response = new APIResponse<object?>();

        RegisterDTOValidator validations = new();

        var result = await validations.ValidateAsync(registerDTO);

        if (!result.IsValid)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.ErrorMessage);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization("InvalidRequest").state;
            return response;
        }

        ApplicationUser appUser = new()
        {
            LastName = registerDTO.LastName,
            UserName = registerDTO.UserName,
            Email = registerDTO.Email,
            IsActive = true
        };

        var identityResult = await _userManager.CreateAsync(appUser, registerDTO.Password);

        if (!identityResult.Succeeded)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in identityResult.Errors)
                stringBuilder.AppendLine(error.Description);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization("Failure").state;
            return response;
        }
        return response;
    }

    public async Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> PasswordResetAsnyc(PasswordResetDTO passwordResetDTO)
    {
        var response = new APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>();

        PasswordResetDTOValidator validations = new();

        var result = await validations.ValidateAsync(passwordResetDTO);

        if (!result.IsValid)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.ErrorMessage);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization("InvalidRequest").state;
            return response;
        }

        ApplicationUser user = await _userManager.FindByEmailAsync(passwordResetDTO.Email);

        if (user is null)
        {
            var msgUserNotFound = GetMessageByLocalization("UserDoesNotFound");
            response.Message = msgUserNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgUserNotFound.state;
            return response;
        }

        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        resetToken = resetToken.UrlEncode();

        await _mailService.SendPasswordResetMailAsync(passwordResetDTO.Email, user.Id.ToString(), resetToken);

        return new APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>();
    }

    public async Task<APIResponse<bool>> VerifyResetTokenAsync(VerifyResetTokenDto verifyResetTokenDto)
    {
        var response = new APIResponse<bool>();

        VerifyResetTokenDTOValidator validations = new();

        var result = await validations.ValidateAsync(verifyResetTokenDto);

        if (!result.IsValid)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.ErrorMessage);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization("InvalidRequest").state;
            return response;
        }

        ApplicationUser user = await _userManager.FindByIdAsync(verifyResetTokenDto.UserId);
        if (user is null)
        {
            var msgUserNotFound = GetMessageByLocalization("UserDoesNotFound");
            response.Message = msgUserNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgUserNotFound.state;
            response.Payload = false;
            return response;
        }

        string resetToken = verifyResetTokenDto.ResetToken.UrlDecode();

        response.Payload = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", resetToken);
        if (!response.Payload)
        {
            var msgInvalidResetToken = GetMessageByLocalization("InvalidResetToken");
            response.Message = msgInvalidResetToken.message;
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.State = msgInvalidResetToken.state;
            return response;
        }

        response.ResponseCode = HttpStatusCode.OK;
        return response;
    }
}
