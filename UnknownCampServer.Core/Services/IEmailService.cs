using UnknownCampServer.Core.Models;

namespace UnknownCampServer.Core.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailRequest emailRequest);
    }
}
