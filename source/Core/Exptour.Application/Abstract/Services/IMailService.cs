using Exptour.Application.DTOs.Mail;
using Exptour.Common.Shared;

namespace Exptour.Application.Abstract.Services;

public interface IMailService
{
    Task<APIResponse<object?>> SendMailAsync(MailRequestDTO mailRequestDTO);
}
