namespace Exptour.Application.DTOs.Auth;

public record SendOTPResponse(DateTime SentAt, DateTime ExpiredAt);
