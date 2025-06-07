using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace RazorPagesEvents.Services
{
    public class EmailSender
    {
        private readonly IConfiguration _config;
        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpServer = _config["Email:SmtpServer"] ?? throw new InvalidOperationException("SMTP server not configured.");
            var smtpPort = _config["Email:SmtpPort"] ?? throw new InvalidOperationException("SMTP port not configured.");
            var smtpUser = _config["Email:Username"] ?? throw new InvalidOperationException("SMTP username not configured.");
            var smtpPass = _config["Email:Password"] ?? throw new InvalidOperationException("SMTP password not configured.");
            var fromEmail = _config["Email:From"] ?? throw new InvalidOperationException("Email 'From' address not configured.");

            var smtpClient = new SmtpClient(smtpServer)
            {
                Port = int.Parse(smtpPort),
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);
            await smtpClient.SendMailAsync(mail);
        }
    }
}