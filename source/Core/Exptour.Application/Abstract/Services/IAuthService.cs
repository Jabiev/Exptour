using Exptour.Application.DTOs.Auth;
using Exptour.Common.Shared;

namespace Exptour.Application.Abstract.Services;

public interface IAuthService
{
    Task<APIResponse<object?>> Register(RegisterDTO registerDTO);
    Task<APIResponse<TokenResponse>> Login(SignInDTO signInDTO);
    Task<APIResponse<TokenResponse>> RefreshToken(string requestRefreshToken);
}
