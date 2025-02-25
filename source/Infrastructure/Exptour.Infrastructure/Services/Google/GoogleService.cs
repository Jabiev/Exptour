using Exptour.Application.Abstract.Services;
using Exptour.Common.Infrastructure.Services;
using Exptour.Common.Shared;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Exptour.Infrastructure.Services.Google;

public class GoogleService : BaseService, IGoogleService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public GoogleService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(httpContextAccessor, configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public async Task<APIResponse<GoogleJsonWebSignature.Payload>> ValidateGoogleTokenAsync(string idToken)
    {
        var response = new APIResponse<GoogleJsonWebSignature.Payload>();

        if (string.IsNullOrEmpty(idToken))
        {
            var msgInvalidToken = GetMessageByLocalization("InvalidToken");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidToken.message;
            response.State = msgInvalidToken.state;
            return response;
        }

        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { _configuration["GoogleSettings:ClientId"] }
        };

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            response.Payload = payload;
            response.ResponseCode = HttpStatusCode.OK;
            return response;
        }
        catch (Exception)
        {
            var msgInvalidToken = GetMessageByLocalization("InvalidToken");
            response.ResponseCode = HttpStatusCode.InternalServerError;
            response.Message = msgInvalidToken.message;
            response.State = msgInvalidToken.state;
            return response;
        }
    }
}
