using Exptour.Application.Abstract.Repositories.Languages;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Languages;

public class LanguageReadRepository : ReadRepository<Language>, ILanguageReadRepository
{
    public LanguageReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}
