using Exptour.Application.DTOs.Role;
using Exptour.Common.Shared;

namespace Exptour.Application.Abstract.Services;

public interface IRoleService
{
    Task<APIResponse<bool>> CreateRoleAsync(string roleName);
    Task<APIResponse<bool>> DeleteRoleAsync(string id);
    Task<APIResponse<Pagination<RoleResponse>>> GetAllRolesAsync(int pageNumber = 1, int take = 10, bool isPaginated = false);
}
