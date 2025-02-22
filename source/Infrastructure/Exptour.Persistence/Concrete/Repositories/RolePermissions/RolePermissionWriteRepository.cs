using Exptour.Application.Abstract.Repositories.RolePermissions;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.RolePermissions;

public class RolePermissionWriteRepository : WriteRepository<RolePermission>, IRolePermissionWriteRepository
{
    public RolePermissionWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}
