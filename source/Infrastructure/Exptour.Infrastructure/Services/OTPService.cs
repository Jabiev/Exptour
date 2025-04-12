using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.Auth;
using Exptour.Application.Settings;
using Exptour.Common;
using Exptour.Common.Helpers;
using Exptour.Common.Infrastructure.Services;
using Exptour.Common.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using static Exptour.Application.Constants.ExceptionMessages;

namespace Exptour.Infrastructure.Services;

public class OTPService : BaseService, IOTPService
{
    private readonly IDistributedCache _cache;
    private readonly IMailService _mailService;
    private readonly OTP _otpSettings;

    public OTPService(IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        IDistributedCache cache,
        IMailService mailService,
        IOptions<OTP> otpSettings) : base(httpContextAccessor, configuration)
    {
        _cache = cache;
        _mailService = mailService;
        _otpSettings = otpSettings.Value;
    }

    public async Task<APIResponse<SendOTPResponse>> SendOTPViaEmailAsync(string email, Language language)
    {
        var response = new APIResponse<SendOTPResponse>();

        if (!email.IsValidEmail())
        {
            var msgInvalidEmail = GetMessageByLocalization(InvalidRequest);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidEmail.message;
            return response;
        }

        try
        {
            string resendKey = $"otp:resend:{email}";
            string otpKey = $"otp:{email}";
            string rateLimitKey = $"otp:rate:{email}";

            if (await _cache.GetStringAsync(resendKey) is not null)
            {
                var msgOTPAlreadySent = GetMessageByLocalization(OTPAlreadySent);
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.Message = msgOTPAlreadySent.message;
                return response;
            }

            var attemptCountStr = await _cache.GetStringAsync(rateLimitKey);
            int attemptCount = string.IsNullOrEmpty(attemptCountStr) ? 0 : int.Parse(attemptCountStr);

            if (attemptCount >= 3)
            {
                var msgRateLimit = GetMessageByLocalization(TooManyRequests);
                response.ResponseCode = HttpStatusCode.TooManyRequests;
                response.Message = msgRateLimit.message;
                return response;
            }

            await _cache.SetStringAsync(rateLimitKey, (attemptCount + 1).ToString(), new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });

            var existingOTPJson = await _cache.GetStringAsync(otpKey);
            if (existingOTPJson is not null)
            {
                var existingOTPData = JsonSerializer.Deserialize<OTPData>(existingOTPJson);
                if (existingOTPData is not null)
                {
                    string subject = Helper.GetByLanguage(_otpSettings.Email.EmailSubjectEn, _otpSettings.Email.EmailSubjectAr, language);
                    string body = Helper.GetByLanguage(_otpSettings.Email.EmailBodyEn, _otpSettings.Email.EmailBodyAr, language) + " " + existingOTPData.OTP;

                    await _mailService.SendMailAsync(new string[] { email }, subject, body);

                    await _cache.SetStringAsync(resendKey, "1", new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_otpSettings.ResendMinute)
                    });

                    response.Payload = new SendOTPResponse
                    (
                        SentAt: DateTime.UtcNow.ToUAE(),
                        ExpiredAt: DateTime.UtcNow.AddMinutes(_otpSettings.ResendMinute).ToUAE()
                    );
                    return response;
                }
            }

            int otp = GenerateOTP();
            var otpData = new OTPData()
            {
                OTP = otp.ToString(),
                Attempt = 0
            };

            var otpJson = JsonSerializer.Serialize(otpData);
            await _cache.SetStringAsync(otpKey, otpJson, new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_otpSettings.ExpireMinute)
            });

            await _cache.SetStringAsync(resendKey, "1", new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_otpSettings.ResendMinute)
            });

            string finalSubject = Helper.GetByLanguage(_otpSettings.Email.EmailSubjectEn, _otpSettings.Email.EmailSubjectAr, language);
            string finalBody = $"{Helper.GetByLanguage(_otpSettings.Email.EmailBodyEn, _otpSettings.Email.EmailBodyAr, language)} {otp}";

            await _mailService.SendMailAsync(new string[] { email }, finalSubject, finalBody);

            response.Payload = new SendOTPResponse
            (
                SentAt: DateTime.UtcNow.ToUAE(),
                ExpiredAt: DateTime.UtcNow.AddMinutes(_otpSettings.ExpireMinute).ToUAE()
            );
            return response;
        }
        catch (Exception ex)
        {
            var msgUnexpectedError = GetMessageByLocalization(SomethingWentWrong);
            response.ResponseCode = HttpStatusCode.InternalServerError;
            response.Message = $"{msgUnexpectedError.message} {ex.Message}";
            return response;
        }
    }

    public async Task<APIResponse<EmptyResult>> VerifyOTPAsync(string email, string otp)
    {
        var response = new APIResponse<EmptyResult>();

        try
        {
            string key = $"otp:{email}";
            var cachedOTPJson = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(cachedOTPJson))
            {
                var msgInvalidOTP = GetMessageByLocalization(InvalidOTP);
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.Message = msgInvalidOTP.message;
                return response;
            }

            var otpData = JsonSerializer.Deserialize<OTPData>(cachedOTPJson);
            if (otpData is null)
            {
                var msgInvalidOTP = GetMessageByLocalization(InvalidOTP);
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.Message = msgInvalidOTP.message;
                return response;
            }

            otpData.Attempt++;
            if (otpData.Attempt >= _otpSettings.AttemptCount)
            {
                await _cache.RemoveAsync(key);
                var msgAttemptExceeded = GetMessageByLocalization(AttemptExceeded);
                response.ResponseCode = HttpStatusCode.BadRequest;
                response.Message = msgAttemptExceeded.message;
                return response;
            }

            if (otpData.OTP == otp)
            {
                await _cache.RemoveAsync(key);

                return response;
            }

            var updatedOTPJson = JsonSerializer.Serialize(otpData);
            await _cache.SetStringAsync(key, updatedOTPJson, new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_otpSettings.ExpireMinute)
            });

            var msgInvalidOTPAgain = GetMessageByLocalization(InvalidOTP);
            response.ResponseCode = HttpStatusCode.Processing;
            response.Message = msgInvalidOTPAgain.message;
            return response;
        }
        catch (Exception ex)
        {
            var msgUnexpectedError = GetMessageByLocalization(SomethingWentWrong);
            response.ResponseCode = HttpStatusCode.InternalServerError;
            response.Message = $"{msgUnexpectedError.message} {ex.Message}";
            return response;
        }
    }

    #region Private

    private class OTPData
    {
        public string OTP { get; set; } = string.Empty;
        public int Attempt { get; set; }
    }

    #endregion

    #region Private Methods

    public int GenerateOTP()
    {
        var otpRangeFrom = _otpSettings.OTPRangeFrom;
        var otpRangeTo = _otpSettings.OTPRangeTo;

        return new Random().Next(otpRangeFrom, otpRangeTo);
    }

    #endregion
}
