namespace Faice_Backend.Interfaces;

public interface IEmailAppService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}
