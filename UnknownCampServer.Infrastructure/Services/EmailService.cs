using UnknownCampServer.Core.Models;
using UnknownCampServer.Core.Repositories;
using UnknownCampServer.Core.Services;

namespace UnknownCampServer.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailRepository _emailRepository;

        public EmailService(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<bool> SendEmailAsync(EmailRequest emailRequest)
        {
            return await _emailRepository.SendEmailAsync(emailRequest);
        }
    }
}
