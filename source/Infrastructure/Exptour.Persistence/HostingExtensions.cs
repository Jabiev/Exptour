using Exptour.Persistence.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Exptour.Persistence;

public static class HostingExtensions
{
    public static void ConfigurePersistenceServices(this IServiceCollection services)
    {
        #region Interceptors

        services.AddScoped<AuditInterceptor>();

        #endregion

        #region Repositories and Services with Assembly Scanning (Scrutor)

        services.Scan(scan => scan
            .FromAssemblies(Assembly.GetExecutingAssembly())
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        #endregion
    }
}
