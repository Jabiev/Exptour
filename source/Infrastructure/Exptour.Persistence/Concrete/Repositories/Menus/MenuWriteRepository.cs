using Exptour.Application.Abstract.Repositories.Menus;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Menus;

public class MenuWriteRepository : WriteRepository<Menu>, IMenuWriteRepository
{
    public MenuWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}
