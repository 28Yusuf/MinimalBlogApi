using TechBlogApi.Services.Abstracts;

namespace TechBlogApi.Services.Concretes
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendBulkNewsletterAsync(List<string> emails, string subject, string body)
        {
            throw new NotImplementedException();
        }

        public Task SendNewsletterEmailAsync(string email, string subject, string body)
        {
            throw new NotImplementedException();
        }
    }
}