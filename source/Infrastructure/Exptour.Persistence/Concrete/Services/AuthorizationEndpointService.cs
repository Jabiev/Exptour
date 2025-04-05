using Exptour.Application.Abstract.Repositories.EndpointRoles;
using Exptour.Application.Abstract.Repositories.Endpoints;
using Exptour.Application.Abstract.Repositories.Menus;
using Exptour.Application.Abstract.Services;
using Exptour.Common.Infrastructure.Services;
using Exptour.Common.Shared;
using Exptour.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using static Exptour.Application.Constants.ExceptionMessages;

namespace Exptour.Persistence.Concrete.Services;

public class AuthorizationEndpointService : BaseService, IAuthorizationEndpointService
{
    private readonly IApplicationService _applicationService;
    private readonly IEndpointReadRepository _endpointReadRepository;
    private readonly IEndpointWriteRepository _endpointWriteRepository;
    private readonly IMenuReadRepository _menuReadRepository;
    private readonly IMenuWriteRepository _menuWriteRepository;
    private readonly IEndpointRoleReadRepository _endpointRoleReadRepository;
    private readonly IEndpointRoleWriteRepository _endpointRoleWriteRepository;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public AuthorizationEndpointService(IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        IApplicationService applicationService,
        IEndpointReadRepository endpointReadRepository,
        IEndpointWriteRepository endpointWriteRepository,
        IMenuReadRepository menuReadRepository,
        IMenuWriteRepository menuWriteRepository,
        RoleManager<IdentityRole<Guid>> roleManager,
        IEndpointRoleReadRepository endpointRoleReadRepository,
        IEndpointRoleWriteRepository endpointRoleWriteRepository) : base(httpContextAccessor, configuration)
    {
        _applicationService = applicationService;
        _endpointReadRepository = endpointReadRepository;
        _endpointWriteRepository = endpointWriteRepository;
        _menuReadRepository = menuReadRepository;
        _menuWriteRepository = menuWriteRepository;
        _roleManager = roleManager;
        _endpointRoleReadRepository = endpointRoleReadRepository;
        _endpointRoleWriteRepository = endpointRoleWriteRepository;
    }

    public async Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> AssignRoleEndpointAsync(string[] roles, string menu, string code, Type type)
    {
        var response = new APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>();

        if (roles is null || roles.Length == 0 || string.IsNullOrEmpty(menu) || string.IsNullOrEmpty(code))
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        Menu _menu = await _menuReadRepository.GetByFiltered(m => m.Name.ToUpper() == menu.ToUpper());
        if (_menu is null)
        {
            _menu = new()
            {
                Id = Guid.NewGuid(),
                Name = menu
            };
            await _menuWriteRepository.AddAsync(_menu);
            await _menuWriteRepository.SaveChangesAsync();
        }

        Endpoint? endpoint = await _endpointReadRepository.Table
            .Include(e => e.Menu)
            .Include(e => e.EndpointRoles)
                .ThenInclude(er => er.Role)
                .FirstOrDefaultAsync(e => e.Code.ToUpper() == code.ToUpper() && e.Menu.Name.ToUpper() == menu.ToUpper());

        if (endpoint is null)
        {
            var action = _applicationService.GetAuthorizeDefinitionEndpoints(type)
                    .Payload
                    .FirstOrDefault(m => m.Name == menu)
                    ?.Actions.FirstOrDefault(e => e.Code.ToUpper() == code.ToUpper());

            if (action is null)
            {
                var msgActionNotFound = GetMessageByLocalization(ActionNotFound);
                response.ResponseCode = HttpStatusCode.NotFound;
                response.Message = msgActionNotFound.message;
                response.State = msgActionNotFound.state;
                return response;
            }

            endpoint = new()
            {
                Code = action.Code,
                ActionType = action.ActionType,
                HttpType = action.HttpType,
                Definition = action.Definition,
                Id = Guid.NewGuid(),
                Menu = _menu
            };

            await _endpointWriteRepository.AddAsync(endpoint);
            await _endpointWriteRepository.SaveChangesAsync();
        }

        var existingRoles = _endpointRoleReadRepository.Where(er => er.EndpointId == endpoint.Id);
        _endpointRoleWriteRepository.RemoveRange(existingRoles);

        var appRoles = await _roleManager.Roles
            .Where(r => roles.Contains(r.Name))
            .ToListAsync();

        if (!appRoles.Any())
        {
            var msgRolesNotFound = GetMessageByLocalization(RolesCanNotBeFound);
            response.ResponseCode = HttpStatusCode.NotFound;
            response.Message = msgRolesNotFound.message;
            response.State = msgRolesNotFound.state;
            return response;
        }

        var newEndpointRoles = appRoles.Select(role => new EndpointRole()
        {
            EndpointId = endpoint.Id,
            RoleId = role.Id
        }).ToList();

        await _endpointRoleWriteRepository.AddRangeAsync(newEndpointRoles);
        await _endpointRoleWriteRepository.SaveChangesAsync();

        return response;
    }

    public async Task<APIResponse<List<string>>> GetRolesAccordingToEndpointAsync(string code, string menu)
    {
        var response = new APIResponse<List<string>>();

        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(menu))
        {
            var msgInvalidRequest = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        Endpoint? endpoint = await _endpointReadRepository.Table
            .Include(e => e.Menu)
            .Include(e => e.EndpointRoles)
                .ThenInclude(er => er.Role)
                .FirstOrDefaultAsync(e => e.Code.ToUpper() == code.ToUpper() && e.Menu.Name.ToUpper() == menu.ToUpper());

        if (endpoint is null)
        {
            var msgNotFound = GetMessageByLocalization(NotFound);
            response.ResponseCode = HttpStatusCode.NotFound;
            response.Message = msgNotFound.message;
            response.State = msgNotFound.state;
            return response;
        }

        var roles = await _endpointRoleReadRepository
            .Where(er => er.EndpointId == endpoint.Id)
            .Select(er => er.Role.Name)
            .ToListAsync();
        response.Payload = roles ?? new List<string>();

        return response;
    }
}
