using MinimalApi.Core;

namespace MinimalApi.Services.Interfaces
{
    public interface IEmailService
    {
        Task<EmailResult> SendEmailAsync(List<string> recipients, string subject, string htmlMessagee, CancellationToken cancellationToken = default);
    }
}
