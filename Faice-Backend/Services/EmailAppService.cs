using Faice_Backend.Interfaces;
using Faice_Backend.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Faice_Backend.Services;

public class EmailAppService(IOptions<SmtpSettings> smtpSettings) : IEmailAppService
{
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port);
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
        client.EnableSsl = true;

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpSettings.SenderEmail),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        await client.SendMailAsync(mailMessage);
    }
}
