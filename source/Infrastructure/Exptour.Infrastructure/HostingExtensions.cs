using Exptour.Application.Abstract.Services;
using Exptour.Infrastructure.Messaging;
using Exptour.Infrastructure.Storage.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Exptour.Infrastructure;

public static class HostingExtensions
{
    public static void ConfigureInfrastructureServices(this IServiceCollection services)
    {
        services.Scan(scan => scan
        .FromAssemblies(Assembly.GetExecutingAssembly())
        .AddClasses(c => c.Where(t =>
            t.Name.EndsWith("Service") &&
            t.GetInterfaces().Any() &&
            !typeof(IHostedService).IsAssignableFrom(t)))
        .AsImplementedInterfaces()
        .WithScopedLifetime());

        services.AddSingleton<IMessageQueueService, RabbitMqService>();
        services.AddHostedService<RabbitMqConsumerService>();
        services.AddScoped<CloudinaryService>();
    }
}
