using Exptour.Common.Infrastructure.Services;
using Exptour.Common.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Exptour.Common.Infrastructure;

public static class HostingExtensions
{
    public static void ConfigureInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IBaseService, BaseService>();
    }
}
