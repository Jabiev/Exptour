using Exptour.Application.Abstract.Services;
using Exptour.Common.Infrastructure.Services.Interfaces;
using Exptour.Infrastructure.ElasticSearch.Services;
using Exptour.Infrastructure.ElasticSearch.Services.Interfaces;
using Exptour.Infrastructure.Google;
using Exptour.Infrastructure.JWT;
using Exptour.Infrastructure.Messaging;
using Exptour.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Exptour.Infrastructure;

public static class HostingExtensions
{
    public static void ConfigureInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IMessageQueueService, RabbitMqService>();
        services.AddHostedService<RabbitMqConsumerService>();
        services.AddScoped<IOTPService, OTPService>();
        services.AddScoped<IJWTService, JWTService>();
        services.AddScoped<IGoogleService, GoogleService>();
        services.AddScoped<IMailService, MailService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<IUserSearchService, UserSearchService>();
        services.AddScoped<IDetectionService, DetectionService>();
    }
}
