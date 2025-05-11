namespace Waslah.Services;

public interface ICustomEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody, string imagePath, string contentId);
}
