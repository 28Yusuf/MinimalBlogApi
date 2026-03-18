using TechBlogApi.Dtos.SocialLink;
using TechBlogApi.Helpers;

namespace TechBlogApi.Services.Abstracts
{
    public interface ISocialService
    {
        Task<ApiResult<IList<SocialDto>>> GetAllSocialsAsync();
        Task<ApiResult<SocialDto>> GetByIdSocialAsync(int id);
        Task<ApiResult> CreateSocial(CreateSocialDto dto);
        Task<ApiResult> UpdateSocial(UpdateSocialDto dto);
        Task<ApiResult> DeleteSocial(int id);
    }
}