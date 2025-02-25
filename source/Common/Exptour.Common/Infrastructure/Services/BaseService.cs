using Exptour.Common.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

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
