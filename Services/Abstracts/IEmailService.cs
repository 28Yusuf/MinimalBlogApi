namespace TechBlogApi.Services.Abstracts
{
    public interface IEmailService
    {
        Task SendNewsletterEmailAsync(string email, string subject, string body);
        Task SendBulkNewsletterAsync(List<string> emails, string subject, string body);
    }
}