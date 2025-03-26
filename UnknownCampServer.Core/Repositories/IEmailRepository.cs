using UnknownCampServer.Core.Models;

namespace UnknownCampServer.Core.Repositories
{
    public interface IEmailRepository
    {
        Task<bool> SendEmailAsync(EmailRequest emailRequest);
    }
}
