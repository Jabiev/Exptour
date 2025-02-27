using Exptour.Application.DTOs.Auth;
using Exptour.Application.DTOs.Mail;
using Exptour.Common.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Exptour.Application.Abstract.Services;

public interface IAuthService
{
    Task<APIResponse<object?>> Register(RegisterDTO registerDTO);
    Task<APIResponse<TokenResponse>> Login(SignInDTO signInDTO);
    Task<APIResponse<TokenResponse>> RefreshToken(string requestRefreshToken);
    Task<APIResponse<EmptyResult>> PasswordResetAsnyc(PasswordResetDTO passwordResetDTO);
    Task<APIResponse<bool>> VerifyResetTokenAsync(VerifyResetTokenDto verifyResetTokenDto);
}
