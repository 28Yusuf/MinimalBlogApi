using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechBlogApi.Models;
using TechBlogApi.Services.Abstracts;
using TechBlogApi.UnitOfWorks;

namespace TechBlogApi.Services.Background
{
    public class NewsletterBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public NewsletterBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken); 
                await SendWeeklyNewsletterAsync();
            }
        }

        private async Task SendWeeklyNewsletterAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var subscribers = await unitOfWork.GetReadRepository<NewsLetter>()
                .GetAllQueryable()
                .Where(x => !x.IsDeleted)
                .Select(x => x.Email).ToListAsync();

            if (subscribers.Any())
            {
                string subject = "";
                string body = "";

                await emailService.SendBulkNewsletterAsync(subscribers, subject, body);
            }
        }
    }
}