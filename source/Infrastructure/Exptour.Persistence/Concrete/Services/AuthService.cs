using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.Auth;
using Exptour.Application.Validators.Auth;
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
using static Exptour.Application.Constants.ExceptionMessages;

namespace Exptour.Persistence.Concrete.Services;

public class AuthService : BaseService, IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJWTService _jWTService;
    private readonly IMailService _mailService;
    private readonly IOTPService _otpService;
    private readonly IConfiguration _configuration;
    private readonly TourismManagementDbContext _tourismManagementDbContext;

    public AuthService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJWTService jWTService,
        IMailService mailService,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        TourismManagementDbContext tourismManagementDbContext,
        IOTPService otpService) : base(httpContextAccessor, configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jWTService = jWTService;
        _mailService = mailService;
        _configuration = configuration;
        _tourismManagementDbContext = tourismManagementDbContext;
        _otpService = otpService;
    }

    public async Task<APIResponse<LoginResponse>> LoginAsync(SignInDTO signInDTO)
    {
        var response = new APIResponse<LoginResponse>();

        var currentLanguage = GetCurrentLanguage();

        SignInDTOValidator validations = new();

        var result = await validations.ValidateAsync(signInDTO);

        if (!result.IsValid)
        {
            StringBuilder stringBuilder = new();

            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.ErrorMessage);

            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization(InvalidRequest).state;
            return response;
        }

        var appUser = await _userManager.FindByEmailAsync(signInDTO.UserNameOrEmail);
        if (appUser is null)
        {
            appUser = await _userManager.FindByNameAsync(signInDTO.UserNameOrEmail);
            if (appUser is null)
            {
                var msgInvalidLogin = GetMessageByLocalization(InvalidLogin);
                response.Message = msgInvalidLogin.message;
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.State = msgInvalidLogin.state;
                return response;
            }
        }

        SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(appUser, signInDTO.Password, true);

        if (!signInResult.Succeeded)
        {
            var msgWrongPassword = GetMessageByLocalization(WrongPassword);
            response.Message = msgWrongPassword.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgWrongPassword.state;
            return response;
        }
        if (!appUser.IsActive)
        {
            var msgUserNotActive = GetMessageByLocalization(UserDoesNotActive);
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

        var sendOtpResponse = await _otpService.SendOTPViaEmailAsync(appUser.Email, currentLanguage);

        response.Payload = new LoginResponse(accessToken,
            refreshToken,
            appUser.RefreshTokenExpiryTime.ToUAE(),
            sendOtpResponse.ResponseCode.ToString());
        response.ResponseCode = HttpStatusCode.OK;
        return response;
    }

    public async Task<APIResponse<TokenResponse>> RefreshTokenAsync(string requestRefreshToken)
    {
        var response = new APIResponse<TokenResponse>();

        var appUser = await _tourismManagementDbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == requestRefreshToken);

        if (appUser is null)
        {
            var msgInvalidRefreshToken = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRefreshToken.message;
            response.State = msgInvalidRefreshToken.state;
            return response;
        }

        if (appUser.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            var msgRefreshTokenExpired = GetMessageByLocalization(RefreshTokenExpired);
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

    public async Task<APIResponse<SendOTPResponse>> SendOTPViaEmailAsync(string email)
    {
        var currentLanguage = GetCurrentLanguage();

        return await _otpService.SendOTPViaEmailAsync(email, currentLanguage);
    }

    public async Task<APIResponse<VerifyOTPResponse>> VerifyOTPAsync(VerifyOTPDTO verifyOTPRequest)
    {
        var response = new APIResponse<VerifyOTPResponse>();

        VerifyOTPDTOValidator validations = new();

        var result = await validations.ValidateAsync(verifyOTPRequest);
        if (!result.IsValid)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.ErrorMessage);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization(InvalidRequest).state;
            return response;
        }

        var verifyOTPResponse = await _otpService.VerifyOTPAsync(verifyOTPRequest.Email, verifyOTPRequest.OTPCode.Trim());

        if (verifyOTPResponse.ResponseCode == HttpStatusCode.OK)
        {
            var appUser = await _userManager.FindByEmailAsync(verifyOTPRequest.Email);
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

            response.Payload = new VerifyOTPResponse(accessToken,
                refreshToken,
                appUser.RefreshTokenExpiryTime.ToUAE(),
                appUser.UserName);
            return response;
        }
        else if (verifyOTPResponse.ResponseCode == HttpStatusCode.Processing)
        {
            var msgPendingVerification = GetMessageByLocalization(PendingVerification);
            response.ResponseCode = HttpStatusCode.Processing;
            response.Message = msgPendingVerification.message;
            response.State = msgPendingVerification.state;
            return response;
        }

        response.ResponseCode = verifyOTPResponse.ResponseCode;
        response.Message = verifyOTPResponse.Message;
        response.State = verifyOTPResponse.State;
        return response;
    }
}
