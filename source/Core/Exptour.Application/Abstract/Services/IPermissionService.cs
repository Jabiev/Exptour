using Exptour.Application.DTOs.Permission;
using Exptour.Common.Shared;

namespace Exptour.Application.Abstract.Services;

public interface IPermissionService
{
    Task<APIResponse<PermissionResponse>> CreatePermissionAsync(CreatePermissionDTO createPermissionDTO);
    Task<APIResponse<List<PermissionResponse>>> GetAllPermissionsAsync();
}
