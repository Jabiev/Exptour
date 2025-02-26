using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.Mail;
using Exptour.Common.Infrastructure.Services;
using Exptour.Common.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Exptour.Persistence.Concrete.Services;

public class MailService : BaseService, IMailService
{
    private readonly IConfiguration _configuration;

    public MailService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(httpContextAccessor, configuration)
    {
        _configuration = configuration;
    }

    public async Task<APIResponse<object?>> SendMailAsync(MailRequestDTO mailRequestDTO)
    {
        var response = new APIResponse<object?>();

        try
        {
            MailMessage mail = new();
            mail.IsBodyHtml = mailRequestDTO.IsBodyHtml;
            foreach (var to in mailRequestDTO.Tos)
                mail.To.Add(to);
            mail.Subject = mailRequestDTO.Subject;
            mail.Body = mailRequestDTO.Body;
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
}
