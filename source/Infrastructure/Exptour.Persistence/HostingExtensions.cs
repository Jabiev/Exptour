using Exptour.Persistence.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace Exptour.Persistence;

public static class HostingExtensions
{
    public static void ConfigurePersistenceServices(this IServiceCollection services)
    {
        services.AddScoped<AuditInterceptor>();
    }
}
