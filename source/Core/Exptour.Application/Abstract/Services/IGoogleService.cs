using Exptour.Application.DTOs.Google;
using Exptour.Common.Shared;

namespace Exptour.Application.Abstract.Services;

public interface IGoogleService
{
    Task<APIResponse<GoogleResponse>> GoogleLoginAsync(string idToken);
}
