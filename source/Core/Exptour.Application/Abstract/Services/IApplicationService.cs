using Exptour.Application.DTOs;
using Exptour.Common.Shared;

namespace Exptour.Application.Abstract.Services;

public interface IApplicationService
{
    APIResponse<List<Menu>> GetAuthorizeDefinitionEndpoints(Type type);
}
