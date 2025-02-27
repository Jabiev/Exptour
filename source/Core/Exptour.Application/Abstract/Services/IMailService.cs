using Exptour.Common.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Exptour.Application.Abstract.Services;

public interface IMailService
{
    Task<APIResponse<EmptyResult>> SendMailAsync(string[] tos, string subject, string body, bool isBodyHtml = true);
    Task<APIResponse<EmptyResult>> SendPasswordResetMailAsync(string to, string userId, string resetToken);
}
