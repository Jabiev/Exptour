using Exptour.Application.Abstract.Repositories.Permissions;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Permissions;

public class PermissionWriteRepository : WriteRepository<Permission>, IPermissionWriteRepository
{
    public PermissionWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}
