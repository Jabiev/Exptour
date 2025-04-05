using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.Role;
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
using static Exptour.Application.Constants.ExceptionMessages;

namespace Exptour.Persistence.Concrete.Services;

public class RoleService : BaseService, IRoleService
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleService(IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<ApplicationUser> userManager) : base(httpContextAccessor, configuration)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<APIResponse<bool>> CreateRoleAsync(string roleName)
    {
        var response = new APIResponse<bool>();

        //if (await _roleManager.RoleExistsAsync(roleName))
        if (await _roleManager.Roles.AnyAsync(r => r.Name.ToLower() == roleName.ToLower()))
        {
            var msgRoleExists = GetMessageByLocalization(RoleAlreadyExists);
            response.Message = msgRoleExists.message;
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.State = msgRoleExists.state;
            return response;
        }

        var result = await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });

        if (!result.Succeeded)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.Description);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization(Failure).state;
            return response;
        }

        response.Payload = true;
        return response;
    }

    public async Task<APIResponse<bool>> DeleteRoleAsync(string id)
    {
        var response = new APIResponse<bool>();

        if (string.IsNullOrEmpty(id) || !id.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var role = await _roleManager.FindByIdAsync(id);
        if (role is null)
        {
            var msgRoleNotFound = GetMessageByLocalization(NotFound);
            response.Message = msgRoleNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgRoleNotFound.state;
            return response;
        }

        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
        if (usersInRole.Any())
        {
            var msgRoleInUse = GetMessageByLocalization(RoleInUse);
            response.Message = msgRoleInUse.message;
            response.ResponseCode = HttpStatusCode.UnprocessableEntity;
            response.State = msgRoleInUse.state;
            return response;
        }

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.Description);
            response.ResponseCode = HttpStatusCode.UnprocessableContent;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization(Failure).state;
            return response;
        }

        response.Payload = true;
        return response;
    }

    public async Task<APIResponse<Pagination<RoleResponse>>> GetAllRolesAsync(int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        var response = new APIResponse<Pagination<RoleResponse>>();

        if (pageNumber < 1 || take < 1)
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var rolesQuery = _roleManager.Roles.AsNoTracking();

        var roles = isPaginated
            ? await rolesQuery
                .Skip((pageNumber - 1) * take)
                .Take(take)
                .Select(x => new RoleResponse(x.Id.ToString(), x.Name))
                .ToListAsync()
            : await rolesQuery
                .Select(x => new RoleResponse(x.Id.ToString(), x.Name))
                .ToListAsync();

        var rolesCount = roles.Count;

        response.Payload = new Pagination<RoleResponse>()
        {
            Items = roles,
            PageIndex = pageNumber,
            PageSize = isPaginated ? take : rolesCount,
            TotalCount = rolesCount,
            TotalPage = isPaginated ? (int)Math.Ceiling((double)rolesCount / take) : 1
        };

        return response;
    }
}
