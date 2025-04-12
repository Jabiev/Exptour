using Exptour.Application.DTOs.Auth;
using Exptour.Common.Shared;

namespace Exptour.Application.Abstract.Services;

public interface IAuthService
{
    Task<APIResponse<LoginResponse>> LoginAsync(SignInDTO signInDTO);
    Task<APIResponse<TokenResponse>> RefreshTokenAsync(string requestRefreshToken);
    Task<APIResponse<SendOTPResponse>> SendOTPViaEmailAsync(string email);
    Task<APIResponse<VerifyOTPResponse>> VerifyOTPAsync(VerifyOTPDTO verifyOTPRequest);
}
