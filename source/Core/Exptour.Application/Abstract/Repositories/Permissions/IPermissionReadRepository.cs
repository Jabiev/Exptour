using Exptour.Domain.Entities;

namespace Exptour.Application.Abstract.Repositories.Permissions;

public interface IPermissionReadRepository : IReadRepository<Permission>
{
    IQueryable<Permission> GetPermissionsForRole(Guid roleId);
}
