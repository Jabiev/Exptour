using Exptour.Application.DTOs.User;
using Exptour.Common.Shared;
using Exptour.Domain.Entities;

namespace Exptour.Application.Abstract.Services;

public interface IUserService
{
    Task<APIResponse<RegisterResponse?>> RegisterAsync(RegisterDTO registerDTO);
    Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> PasswordResetAsnyc(PasswordResetDTO passwordResetDTO);
    Task<APIResponse<bool>> VerifyResetTokenAsync(VerifyResetTokenDto verifyResetTokenDto);
    Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> UpdatePasswordAsync(string id, UpdatePasswordDTO updatePasswordDTO);
    Task<APIResponse<UpdateProfileResponse>> UpdateProfileAsync(UpdateProfileDTO updateProfileDTO);
    Task<APIResponse<Pagination<UserResponse>>> GetAllUsersAsync(int pageNumber = 1, int take = 10, bool isPaginated = false);
    Task<APIResponse<Pagination<UserResponse>>> SearchAsync(string search, int pageNumber = 1, int take = 10, bool isPaginated = false);
    Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> AssignRoleToUserAsnyc(AssignRoleDTO assignRoleDTO);
    Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> RemoveRoleFromUserAsync(string userId, string[] roles);
    Task<APIResponse<List<string>>> GetRolesAccordingToUserAsync(string userIdOrName);
    //Task ExistingUsersToElasticAsync();
    //Task AddUserToElasticSearchAsync(ApplicationUser user);
    Task<APIResponse<bool>> HasRolePermissionToEndpointAsync(string name, string code);
    Task<string?> GetUserIdByEmailAsync(string email);
    Task<string?> GetGuideIdByUserIdAsync(string userId);
}
