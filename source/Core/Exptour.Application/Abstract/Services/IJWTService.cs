using System.Security.Claims;

namespace Exptour.Application.Abstract.Services;

public interface IJWTService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
}
