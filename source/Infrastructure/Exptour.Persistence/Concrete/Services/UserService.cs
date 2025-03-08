using Exptour.Application.Abstract.Repositories.Endpoints;
using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.User;
using Exptour.Application.Validators.User;
using Exptour.Common.Helpers;
using Exptour.Common.Infrastructure.Services;
using Exptour.Common.Shared;
using Exptour.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;

namespace Exptour.Persistence.Concrete.Services;

public class UserService : BaseService, IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IEndpointReadRepository _endpointReadRepository;

    public UserService(IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager) : base(httpContextAccessor, configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> UpdatePasswordAsync(UpdatePasswordDTO updatePasswordDTO)
    {
        var response = new APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>();

        UpdatePasswordDTOValidator validations = new();

        var result = await validations.ValidateAsync(updatePasswordDTO);

        if (!result.IsValid)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.ErrorMessage);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization("InvalidRequest").state;
            return response;
        }

        ApplicationUser user = await _userManager.FindByIdAsync(updatePasswordDTO.UserId);
        if (user is null)
        {
            var msgUserNotFound = GetMessageByLocalization("UserDoesNotFound");
            response.Message = msgUserNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgUserNotFound.state;
            return response;
        }

        string resetToken = updatePasswordDTO.ResetToken.UrlDecode();
        IdentityResult res = await _userManager.ResetPasswordAsync(user, resetToken, updatePasswordDTO.NewPassword);
        if (!res.Succeeded)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in res.Errors)
                stringBuilder.AppendLine(error.Description);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization("Failure").state;
            return response;
        }

        await _userManager.UpdateSecurityStampAsync(user);
        return response;
    }

    public async Task<APIResponse<bool>> UpdateProfileAsync(UpdateProfileDTO updateProfileDTO)
    {
        var response = new APIResponse<bool>();

        if (string.IsNullOrEmpty(updateProfileDTO.UserId) || updateProfileDTO.UserId.IsValidGuid())
        {
            var msgUserIdInvalid = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgUserIdInvalid.message;
            response.State = msgUserIdInvalid.state;
            return response;
        }

        ApplicationUser user = await _userManager.FindByIdAsync(updateProfileDTO.UserId);
        if (user is null)
        {
            var msgUserNotFound = GetMessageByLocalization("UserDoesNotFound");
            response.Message = msgUserNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgUserNotFound.state;
            return response;
        }

        user.UserName = updateProfileDTO.UserName;
        user.LastName = updateProfileDTO.LastName;
        user.PhoneNumber = updateProfileDTO.PhoneNumber;
        user.Email = updateProfileDTO.Email;

        IdentityResult result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.Description);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization("Failure").state;
            return response;
        }

        response.Payload = true;
        return response;
    }

    public async Task<APIResponse<Pagination<UserResponse>>> GetAllUsersAsync(int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        var response = new APIResponse<Pagination<UserResponse>>();

        if (pageNumber < 1 || take < 1)
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var users = isPaginated
            ? _userManager.Users
                .Skip((pageNumber - 1) * take)
                .Take(take)
            : _userManager.Users;

        var usersCount = await users.CountAsync();

        response.Payload = new Pagination<UserResponse>()
        {
            Items = await users.Select(x => new UserResponse(x.Id, x.Email, x.FullName)).ToListAsync(),
            PageIndex = pageNumber,
            PageSize = isPaginated ? take : usersCount,
            TotalCount = usersCount,
            TotalPage = isPaginated ? (int)Math.Ceiling((double)usersCount / take) : 1
        };
        return response;
    }

    public async Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> AssignRoleToUserAsnyc(AssignRoleDTO assignRoleDTO)
    {
        var response = new APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>();

        AssignRoleDTOValidator validations = new();

        var res = await validations.ValidateAsync(assignRoleDTO);

        if (!res.IsValid)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in res.Errors)
                stringBuilder.AppendLine(error.ErrorMessage);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization("InvalidRequest").state;
            return response;
        }

        ApplicationUser user = await _userManager.FindByIdAsync(assignRoleDTO.UserId);
        if (user is null)
        {
            var msgUserNotFound = GetMessageByLocalization("UserDoesNotFound");
            response.Message = msgUserNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgUserNotFound.state;
            return response;
        }

        foreach (var role in assignRoleDTO.Roles)
        {
            if (!(await _roleManager.RoleExistsAsync(role)))
            {
                var msgRoleNotFound = GetMessageByLocalization("RoleDoesNotExist");
                response.Message = string.Format(msgRoleNotFound.message, role);
                response.ResponseCode = HttpStatusCode.NotFound;
                response.State = msgRoleNotFound.state;
                return response;
            }
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var rolesToAdd = assignRoleDTO.Roles.Except(userRoles);
        if (!rolesToAdd.Any())
        {
            var msgNoNewRoles = GetMessageByLocalization("NoNewRolesToAssign");
            response.Message = msgNoNewRoles.message;
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.State = msgNoNewRoles.state;
            return response;
        }

        var result = await _userManager.AddToRolesAsync(user, rolesToAdd);
        if (!result.Succeeded)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.Description);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization("Failure").state;
            return response;
        }

        return response;
    }

    public async Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> RemoveRoleFromUserAsync(string userId, string[] roles)
    {
        var response = new APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>();

        if (string.IsNullOrEmpty(userId) || !userId.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        ApplicationUser user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            var msgUserNotFound = GetMessageByLocalization("UserDoesNotFound");
            response.Message = msgUserNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgUserNotFound.state;
            return response;
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        if (!userRoles.Any())
        {
            var msgNoRolesAssigned = GetMessageByLocalization("NoRolesAssigned");
            response.Message = msgNoRolesAssigned.message;
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.State = msgNoRolesAssigned.state;
            return response;
        }

        var rolesToRemove = roles.Intersect(userRoles);
        if (!rolesToRemove.Any())
        {
            var msgRolesNotFound = GetMessageByLocalization("NotFound");
            response.Message = msgRolesNotFound.message;
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.State = msgRolesNotFound.state;
            return response;
        }

        var result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

        if (!result.Succeeded)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.Description);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization("Failure").state;
            return response;
        }

        return response;
    }

    public async Task<APIResponse<List<string>>> GetRolesAccordingToUserAsync(string userIdOrName)
    {
        var response = new APIResponse<List<string>>();

        ApplicationUser user = null;

        if (userIdOrName.IsValidGuid())
            user = await _userManager.FindByIdAsync(userIdOrName);

        if (user is null)
            user = await _userManager.FindByNameAsync(userIdOrName);


        if (user is null)
        {
            var msgUserNotFound = GetMessageByLocalization("UserDoesNotFound");
            response.ResponseCode = HttpStatusCode.NotFound;
            response.Message = msgUserNotFound.message;
            response.State = msgUserNotFound.state;
            return response;
        }

        response.Payload = (await _userManager.GetRolesAsync(user)).ToList();
        return response;
    }

    public async Task<APIResponse<bool>> HasRolePermissionToEndpointAsync(string name, string code)
    {
        var response = new APIResponse<bool>();

        var userRoles = await GetRolesAccordingToUserAsync(name);

        if (userRoles.Payload is null || !userRoles.Payload.Any())
        {
            var msgUserNotFound = GetMessageByLocalization("UserDoesNotFound");
            response.ResponseCode = HttpStatusCode.NotFound;
            response.Message = msgUserNotFound.message;
            response.State = msgUserNotFound.state;
            return response;
        }

        Endpoint? endpoint = await _endpointReadRepository.Table
            .Include(e => e.Menu)
            .Include(e => e.EndpointRoles)
                .ThenInclude(er => er.Role)
                .FirstOrDefaultAsync(e => e.Code == code);

        if (endpoint is null)
        {
            var msgEndpointNotFound = GetMessageByLocalization("NotFound");
            response.Message = msgEndpointNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgEndpointNotFound.state;
            response.Payload = false;
            return response;
        }

        var endpointRoles = endpoint.EndpointRoles.Select(er => er.Role.Name);

        response.Payload = userRoles.Payload.Any(userRole => endpointRoles.Contains(userRole));
        return response;
    }
}