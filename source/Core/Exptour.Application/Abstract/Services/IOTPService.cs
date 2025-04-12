using Exptour.Application.DTOs.Auth;
using Exptour.Common;
using Exptour.Common.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Exptour.Application.Abstract.Services;

public interface IOTPService
{
    Task<APIResponse<SendOTPResponse>> SendOTPViaEmailAsync(string email, Language language);
    Task<APIResponse<EmptyResult>> VerifyOTPAsync(string email, string otp);
}
