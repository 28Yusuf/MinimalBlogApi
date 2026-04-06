using Microsoft.EntityFrameworkCore;
using TechBlogApi.Helpers;
using TechBlogApi.Models;
using TechBlogApi.Services.Abstracts;
using TechBlogApi.UnitOfWorks;

namespace TechBlogApi.Services.Concretes
{
    public class NewsLetterService : INewsLetterService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NewsLetterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult> SubscribeAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return new ApiResult(false, "Email is required");

            var existing = await _unitOfWork.GetReadRepository<NewsLetter>()
                .GetAsync(x => x.Email == email && !x.IsDeleted);

            if (existing != null)
                return new ApiResult(false, "This email is already subscribed");

            var newsletter = new NewsLetter
            {
                Email = email,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.GetWriteRepository<NewsLetter>().AddAsync(newsletter);
            int result = await _unitOfWork.SaveAsync();

            return result > 0
                ? new ApiResult(true, "Successfully subscribed to newsletter")
                : new ApiResult(false, "Failed to subscribe");
        }

        public async Task<ApiResult> UnsubscribeAsync(string email)
        {
            var newsletter = await _unitOfWork.GetReadRepository<NewsLetter>()
                .GetAsync(x => x.Email == email && !x.IsDeleted);

            if (newsletter == null)
                return new ApiResult(false, "Email not found");

            await _unitOfWork.GetWriteRepository<NewsLetter>().SoftDeleteAsync(newsletter.Id);
            int result = await _unitOfWork.SaveAsync();

            return result > 0
                ? new ApiResult(true, "Successfully unsubscribed from newsletter")
                : new ApiResult(false, "Failed to unsubscribe");
        }

        public async Task<ApiResult> GetAllSubscribersAsync()
        {
            var subscribers = await _unitOfWork.GetReadRepository<NewsLetter>()
                .GetAllQueryable()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new { x.Id, x.Email, x.CreatedDate })
                .ToListAsync();

            return new ApiResult(true, "");
        }

        public async Task<ApiResult> IsSubscribedAsync(string email)
        {
            var exists = await _unitOfWork.GetReadRepository<NewsLetter>()
                .GetAsync(x => x.Email == email);
            if (exists != null)
                return new ApiResult(true, "");
            return new ApiResult(false, "Not exisits");
        }
    }
}