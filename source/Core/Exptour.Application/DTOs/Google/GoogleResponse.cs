using Google.Apis.Auth;

namespace Exptour.Application.DTOs.Google;

public record GoogleResponse(GoogleJsonWebSignature.Payload Payload, string AccessToken, string RefreshToken, DateTime Expiration);
