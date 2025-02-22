using Exptour.Application.Abstract.Repositories.Permissions;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Permissions;

public class PermissionReadRepository : ReadRepository<Permission>, IPermissionReadRepository
{
    public PermissionReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }

    public IQueryable<Permission> GetPermissionsForRole(Guid roleId)
        => _tourismManagementDbContext.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.Permission);
}
