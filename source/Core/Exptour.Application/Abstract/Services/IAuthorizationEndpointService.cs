using Exptour.Common.Shared;

namespace Exptour.Application.Abstract.Services;

public interface IAuthorizationEndpointService
{
    public Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> AssignRoleEndpointAsync(string[] roles, string menu, string code, Type type);
    public Task<APIResponse<List<string>>> GetRolesAccordingToEndpointAsync(string code, string menu);
}
