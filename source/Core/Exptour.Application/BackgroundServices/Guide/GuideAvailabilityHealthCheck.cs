using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Exptour.Application.BackgroundServices.Guide;

public class GuideAvailabilityHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(HealthCheckResult.Healthy("GuideScheduleBackgroundService is running"));
    }
}
