namespace Exptour.Application.DTOs.Mail;

public record MailRequestDTO(string[] Tos, string Subject, string Body, bool IsBodyHtml = true);
