using Exptour.Application.Abstract.Repositories.Guides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Exptour.Application.BackgroundServices.Guide;

public class GuideAvailabilityBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<GuideAvailabilityBackgroundService> _logger;

    public GuideAvailabilityBackgroundService(IServiceScopeFactory serviceScopeFactory,
        ILogger<GuideAvailabilityBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromHours(3));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _guideReadRepository = scope.ServiceProvider.GetRequiredService<IGuideReadRepository>();

                    var now = DateTime.UtcNow.AddDays(-1);

                    var updatedGuidesCount = await _guideReadRepository.Table
                    .Where(g => g.GuideSchedules.Any()
                        && g.Status == Domain.GuideStatus.Active
                        && !g.IsAvailable
                        && !g.GuideSchedules.Any(a => a.StartDate <= now && a.EndDate >= now)
                        && g.GuideSchedules.Max(a => a.EndDate) <= now.AddDays(-2))
                    .ExecuteUpdateAsync(setters => setters.SetProperty(g => g.IsAvailable, true));

                    _logger.LogInformation($"Setting {updatedGuidesCount} guides as available");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while setting guides as available");
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("GuideScheduleBackgroundService is stopping.");

        return base.StopAsync(cancellationToken);
    }
}
