using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.Mail;
using Exptour.Common.Infrastructure.Services;
using Exptour.Common.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Exptour.Infrastructure.Services;

public class MailService : BaseService, IMailService
{
    private readonly IConfiguration _configuration;

    public MailService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(httpContextAccessor, configuration)
    {
        _configuration = configuration;
    }

    public async Task<APIResponse<EmptyResult>> SendMailAsync(string[] tos, string subject, string body, bool isBodyHtml = true)
    {
        var response = new APIResponse<EmptyResult>();

        try
        {
            MailMessage mail = new();
            mail.IsBodyHtml = isBodyHtml;
            foreach (var to in tos)
                mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;
            mail.From = new(_configuration["Mail:Username"], _configuration["Mail:DisplayName"], System.Text.Encoding.UTF8);

            using SmtpClient smtp = new()
            {
                Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]),
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Host = _configuration["Mail:Host"]
            };

            await smtp.SendMailAsync(mail);

            response.ResponseCode = HttpStatusCode.OK;
            response.Message = GetMessageByLocalization("MailSentSuccessfully").message;
            response.State = GetMessageByLocalization("Success").state;
        }
        catch (Exception ex)
        {
            response.ResponseCode = HttpStatusCode.InternalServerError;
            response.Message = GetMessageByLocalization("MailCanNotSent").message;
            response.State = GetMessageByLocalization("Failure").state;
            response.ErrorDetails = new Dictionary<string, string> { { "Exception", ex.Message } };
        }
        return response;
    }

    public async Task<APIResponse<EmptyResult>> SendPasswordResetMailAsync(string to, string userId, string resetToken)
    {
        StringBuilder mail = new();
        mail.AppendLine("Hello<br>If you have requested a new password, you can renew your password from the link below.<br><strong><a target=\"_blank\" href=\"");
        mail.AppendLine(_configuration["ApplicationUrl"]);
        mail.AppendLine("/update-password/");
        mail.AppendLine(userId);
        mail.AppendLine("/");
        mail.AppendLine(resetToken);
        mail.AppendLine("\">Click to request a new password...</a></strong><br><br><span style=\"font-size:12px;\">NOT : If this request has not been fulfilled by you, please do not take this e-mail seriously.</span><br>Best Regards...<br><br><br>Exp Tour ---    by Jabiev");

        return await SendMailAsync(new string[] { to }, "Password Reset Request", mail.ToString());
    }
}
