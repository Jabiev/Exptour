using Exptour.Application.DTOs.User;
using Exptour.Common.Shared;

namespace Exptour.Application.Abstract.Services;

public interface IUserService
{
    Task<APIResponse<Microsoft.AspNetCore.Mvc.EmptyResult>> UpdatePasswordAsync(UpdatePasswordDTO updatePasswordDTO);
}
