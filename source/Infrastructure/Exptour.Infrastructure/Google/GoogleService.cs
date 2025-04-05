using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.Google;
using Exptour.Application.DTOs.User;
using Exptour.Common.Helpers;
using Exptour.Common.Infrastructure.Services;
using Exptour.Common.Shared;
using Exptour.Domain.Entities;
using Exptour.Domain.Events;
using Exptour.Infrastructure.Services;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Security.Claims;
using System.Text;
using static Exptour.Application.Constants.ExceptionMessages;

namespace Exptour.Infrastructure.Google;

public class GoogleService : BaseService, IGoogleService
{
    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator;
    private readonly IJWTService _jWTService;
    private readonly IMailService _mailService;
    private readonly UserManager<ApplicationUser> _userManager;

    public GoogleService(IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        IMediator mediator,
        IJWTService jWTService,
        IMailService mailService,
        UserManager<ApplicationUser> userManager) : base(httpContextAccessor, configuration)
    {
        _configuration = configuration;
        _mediator = mediator;
        _jWTService = jWTService;
        _mailService = mailService;
        _userManager = userManager;
    }

    public async Task<APIResponse<GoogleResponse>> GoogleLoginAsync(string idToken)
    {
        var response = new APIResponse<GoogleResponse>();

        var result = await ValidateGoogleTokenAsync(idToken);
        if (result.ResponseCode != HttpStatusCode.OK)
        {
            response.ResponseCode = result.ResponseCode;
            response.Message = result.Message;
            response.State = result.State;
            return response;
        }

        ApplicationUser? appUser = await _userManager.FindByEmailAsync(result.Payload.Email);
        if (appUser is null)
        {
            appUser = new ApplicationUser()
            {
                LastName = result.Payload.Email.GetNameFromEmail().ToProperLastName(),
                UserName = result.Payload.Email.GetNameFromEmail().ToProperFirstName(),
                Email = result.Payload.Email,
                Gender = Domain.Gender.RatherNotSay,
                IsActive = true,
            };

            var identityResult = await _userManager.CreateAsync(appUser);

            if (!identityResult.Succeeded)
            {
                StringBuilder stringBuilder = new();
                foreach (var error in identityResult.Errors)
                    stringBuilder.AppendLine(error.Description);
                response.ResponseCode = HttpStatusCode.UnprocessableContent;
                response.Message = stringBuilder.ToString();
                response.State = GetMessageByLocalization(Failure).state;
                return response;
            }

            var @event = new UserEvent(appUser.Id, appUser.Email, appUser.FullName, new List<string>());
            await _mediator.Publish(@event);

            await _mailService.SendMailAsync(new string[] { appUser.Email }, $"Welcome to {_configuration["Mail:DisplayName"]}", @$"Dear {appUser.UserName},
            We are excited to welcome you to {_configuration["Mail:DisplayName"]}!
            Your registration was successful, and you can now start exploring our platform.
            If you have any questions or need assistance, feel free to contact our support team.
            <br>Best Regards...<br><br><br> {_configuration["Mail:DisplayName"]} Team");
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

        response.Payload = new GoogleResponse(result.Payload, accessToken, refreshToken, appUser.RefreshTokenExpiryTime.ToUAE());
        return response;
    }

    #region Private Methods

    private async Task<APIResponse<GoogleJsonWebSignature.Payload>> ValidateGoogleTokenAsync(string idToken)
    {
        var response = new APIResponse<GoogleJsonWebSignature.Payload>();

        if (string.IsNullOrEmpty(idToken))
        {
            var msgInvalidToken = GetMessageByLocalization(InvalidToken);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidToken.message;
            response.State = msgInvalidToken.state;
            return response;
        }

        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { _configuration["GoogleSettings:ClientId"] }
        };

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
        catch (Exception)
        {
            var msgInvalidToken = GetMessageByLocalization(InvalidToken);
            response.ResponseCode = HttpStatusCode.InternalServerError;
            response.Message = msgInvalidToken.message;
            response.State = msgInvalidToken.state;
            return response;
        }

        response.Payload = payload;
        return response;
    }

    #endregion

}
