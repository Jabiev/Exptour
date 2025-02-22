using Exptour.Application.Abstract.Repositories.Languages;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;


namespace Exptour.Persistence.Concrete.Repositories.Languages;

public class LanguageWriteRepository : WriteRepository<Language>, ILanguageWriteRepository
{
    public LanguageWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}
