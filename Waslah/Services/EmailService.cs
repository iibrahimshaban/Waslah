using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Waslah.Settings;

namespace Waslah.Services;

public class EmailService(IOptions<MailSettings> options) : ICustomEmailService
{
    private readonly MailSettings _mailSettings = options.Value;

    public async Task SendEmailAsync(string Email, string subject, string htmlMessage, string embeddedImagePath, string embeddedImageCid)
    {
        var message = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_mailSettings.Mail),
            Subject = subject
        };

        message.To.Add(MailboxAddress.Parse(Email));

        var builder = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };

        var image = builder.LinkedResources.Add(embeddedImagePath);
        image.ContentId = embeddedImageCid;

        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        smtp.CheckCertificateRevocation = false;
        smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

        await smtp.SendAsync(message);

        smtp.Disconnect(true);
    }
}
