using Exptour.Application.Abstract.Repositories.Menus;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Menus;

public class MenuReadRepository : ReadRepository<Menu>, IMenuReadRepository
{
    public MenuReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}
