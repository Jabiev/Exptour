namespace Exptour.Application.DTOs.Auth;

public record TokenResponse(string AccessToken, string RefreshToken, DateTime Expiration);
