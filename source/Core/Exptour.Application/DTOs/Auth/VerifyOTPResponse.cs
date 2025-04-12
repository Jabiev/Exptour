namespace Exptour.Application.DTOs.Auth;

public record VerifyOTPResponse(string AccessToken, string RefreshToken, DateTime Expiration, string FirstName);
