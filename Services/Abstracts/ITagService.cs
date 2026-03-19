using TechBlogApi.Dtos.Tag;
using TechBlogApi.Helpers;

namespace TechBlogApi.Services.Abstracts
{
    public interface ITagService
    {
        Task<ApiResult<IList<TagDto>>> GetAllTagsAsync();
        Task<ApiResult<TagDto>> GetByIdTagAsync(int id);
        Task<ApiResult> CreateTagAsync(CreateTagDto dto);
        Task<ApiResult> UpdateTagAsync(UpdateTagDto dto);
        Task<ApiResult> DeleteTagAsync(int id);
    }
}