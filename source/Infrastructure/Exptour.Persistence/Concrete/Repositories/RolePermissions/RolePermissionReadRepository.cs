using Exptour.Application.Abstract.Repositories.RolePermissions;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.RolePermissions;

public class RolePermissionReadRepository : ReadRepository<RolePermission>, IRolePermissionReadRepository
{
    public RolePermissionReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}
