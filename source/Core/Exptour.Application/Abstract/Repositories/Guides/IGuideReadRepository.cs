using Exptour.Domain.Entities;

namespace Exptour.Application.Abstract.Repositories.Guides;

public interface IGuideReadRepository : IReadRepository<Guide>
{
    IQueryable<Guide> GetAvailableGuides(DateTime start, DateTime end, string language);
}
