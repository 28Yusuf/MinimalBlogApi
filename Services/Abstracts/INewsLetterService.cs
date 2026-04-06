using TechBlogApi.Helpers;

namespace TechBlogApi.Services.Abstracts
{
    public interface INewsLetterService
    {
        Task<ApiResult> SubscribeAsync(string email);
        Task<ApiResult> UnsubscribeAsync(string email);
        Task<ApiResult> GetAllSubscribersAsync();
        Task<ApiResult> IsSubscribedAsync(string email);
    }
}