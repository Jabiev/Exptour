using Exptour.Common.Shared;
using Google.Apis.Auth;

namespace Exptour.Application.Abstract.Services;

public interface IGoogleService
{
    Task<APIResponse<GoogleJsonWebSignature.Payload>> ValidateGoogleTokenAsync(string idToken);
}
