namespace Exptour.Application.DTOs.Auth;

public record LoginResponse(string AccessToken, string RefreshToken, DateTime Expiration, string OTPSendStatus);
