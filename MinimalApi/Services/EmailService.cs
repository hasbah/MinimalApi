using MinimalApi.Services.Interfaces; 
using MimeKit; 
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Xml.Linq;
using MinimalApi.Core;

namespace MinimalApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private string emailSenderLogin;
        private string emailSenderSmtpServer;
        private int emailSenderPort;
        private bool emailSenderEnableSsl;
        private string emailSenderPassword;

        public EmailService(IConfiguration configuration,
            ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
#nullable disable
            emailSenderLogin        = _configuration.GetValue<string>("EmailSenderLogin");
            emailSenderSmtpServer   = _configuration.GetValue<string>("EmailSenderSmtpServer");
            emailSenderPort         = _configuration.GetValue<int>("EmailSenderPort");
            emailSenderEnableSsl    = _configuration.GetValue<bool>("EmailSenderEnableSsl");
            emailSenderPassword     = _configuration.GetValue<string>("EmailSenderPassword");
#nullable enable
        }

        public async Task<EmailResult> SendEmailAsync(List<string> recipients, string subject, string htmlMessage, CancellationToken cancellationToken = default)
        {
            var emailResult = new EmailResult() { IsSuccess = true };

            foreach (var recipient in recipients)
            {
                try
                {
                    _logger.LogInformation($"Sending email to '{recipient}': {subject}"); 

                    var email = new MimeMessage();
                    email.From.Add(new MailboxAddress(emailSenderLogin, emailSenderLogin));
                    email.To.Add(new MailboxAddress(recipient, recipient));
                    email.Subject = subject;
                    email.Body = new TextPart(TextFormat.Html)
                    {
                        Text = htmlMessage
                    };

                    using var client = new SmtpClient();
                    await client.ConnectAsync(emailSenderSmtpServer, emailSenderPort, SecureSocketOptions.None, cancellationToken);

                    //await client.AuthenticateAsync(emailSenderLogin, emailSenderPassword);
                    await client.SendAsync(email, cancellationToken);
                    await client.DisconnectAsync(true, cancellationToken);  

                }
                catch (Exception ex)
                {
                    emailResult.IsSuccess = false;
                    emailResult.ErrorMessage.Add($"Can't send email to {recipient}");
                    _logger.LogError(ex, $"Can't send email to '{recipient}': {subject}\n{htmlMessage} - {ex.Message}");
                }
            }

            return emailResult;
        }
    }
}
