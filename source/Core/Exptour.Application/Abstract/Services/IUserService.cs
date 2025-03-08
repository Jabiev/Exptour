using Exptour.Application.DTOs.User;
using Exptour.Common.Shared;

namespace Exptour.Application.Abstract.Services;

public interface IUserService
{
    Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> UpdatePasswordAsync(UpdatePasswordDTO updatePasswordDTO);
    Task<APIResponse<bool>> UpdateProfileAsync(UpdateProfileDTO updateProfileDTO);
    Task<APIResponse<Pagination<UserResponse>>> GetAllUsersAsync(int pageNumber = 1, int take = 10, bool isPaginated = false);
    Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> AssignRoleToUserAsnyc(AssignRoleDTO assignRoleDTO);
    Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> RemoveRoleFromUserAsync(string userId, string[] roles);
    Task<APIResponse<List<string>>> GetRolesAccordingToUserAsync(string userIdOrName);
    Task<APIResponse<bool>> HasRolePermissionToEndpointAsync(string name, string code);
}
