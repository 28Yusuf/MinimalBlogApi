using TechBlogApi.Dtos.Category;
using TechBlogApi.Helpers;

namespace TechBlogApi.Services.Abstracts
{
    public interface ICategoryService
    {
        Task<ApiResult<IEnumerable<CategoryDto>>> GetAllCategoryAsync();
        Task<ApiResult<CategoryDto>> GetAsyncCategory(int id);
        Task<ApiResult> CreateCategoryAsync(CreateCategoryDto dto);
        Task<ApiResult> UpdateCategoryAsync(UpdateCategoryDto dto);
        Task<ApiResult> DeleteCategoryAsync(int id);
    }
}