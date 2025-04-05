using Exptour.Common.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Exptour.Common.Infrastructure.Services;

public class BaseService : IBaseService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public BaseService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public string GetHeaderByName(string headerName)
    {
        var value = _httpContextAccessor.HttpContext?.Request?.Headers[headerName];
        value = value ?? Convert.ToString(value);
        return value;
    }

    public string? GetAuthToken()
    {
        string? authToken = _httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Authorization];
        if (authToken is not null)
            authToken = authToken.Replace("Bearer ", "");
        return authToken;
    }

    public (string authId, string authRole) GetAuthData()
    {
        var authId = string.Empty;
        var authRole = string.Empty;
        string? authToken = GetAuthToken();
        if (!string.IsNullOrEmpty(authToken))
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(authToken) as JwtSecurityToken;

            var userId = securityToken?.Claims.FirstOrDefault(claim => claim.Type is "sub"
                or ClaimTypes.NameIdentifier
                or "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? string.Empty;
            authId = userId;

            var role = securityToken?.Claims.FirstOrDefault(claim => claim.Type is "role"
                or ClaimTypes.Role
                or "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value ?? string.Empty;
            authRole = role;
        }

        return (authId, authRole);
    }

    public Language GetCurrentLanguage()
    {
        var acceptLanguage = GetHeaderByName("Accept-Language") ?? "en";
        return acceptLanguage == "en" || acceptLanguage != "ar" ? Language.English : Language.Arabic;
    }

    public (string message, string state) GetMessageByLocalization(string key)
    {
        var acceptLanguage = GetHeaderByName("Accept-Language");
        acceptLanguage = !String.IsNullOrEmpty(acceptLanguage) && (acceptLanguage == "en" || acceptLanguage == "ar") ? GetHeaderByName("Accept-Language") : "en";
        string localizedMessage = _configuration[$"AppMessages:{key}:{acceptLanguage}"];
        string state = _configuration[$"AppMessages:{key}:state"];
        if (string.IsNullOrEmpty(localizedMessage))
            return (key, "");
        return (localizedMessage, state);
    }
}
