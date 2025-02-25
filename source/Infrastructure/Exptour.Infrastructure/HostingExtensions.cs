using Exptour.Application.Abstract.Services;
using Exptour.Infrastructure.Services.Google;
using Exptour.Infrastructure.Services.JWT;
using Microsoft.Extensions.DependencyInjection;

namespace Exptour.Infrastructure;

public static class HostingExtensions
{
    public static void ConfigureInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IJWTService, JWTService>();
        services.AddScoped<IGoogleService, GoogleService>();
    }
}
