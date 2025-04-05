using Exptour.Application.Abstract.Repositories.Guides;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Exptour.Application.BackgroundJobs.Guide;

public class GuideAvailabilityJob : IJob
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public GuideAvailabilityJob(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var _guideReadRepository = scope.ServiceProvider.GetRequiredService<IGuideReadRepository>();
            var _guideWriteRepository = scope.ServiceProvider.GetRequiredService<IGuideWriteRepository>();

            var guideId = context.JobDetail.JobDataMap.GetString("GuideId");
            if (string.IsNullOrEmpty(guideId))
                return;

            var guide = await _guideReadRepository.FirstOrDefaultAsync(g => g.Id == Guid.Parse(guideId) && !g.IsDeleted);
            if (guide is null)
                return;

            guide.IsAvailable = false;
            _guideWriteRepository.Update(guide);
            await _guideWriteRepository.SaveChangesAsync();
        }
    }
}
