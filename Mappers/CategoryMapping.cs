
using TechBlogApi.Dtos.Category;
using TechBlogApi.Models;

namespace TechBlogApi.Mappers
{
    public static class CategoryMapping
    {
        public static CategoryDto ToDto(this Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                CreatedBy = category.CreatedBy,
                UpdatedBy = category.UpdatedBy,
                CreatedDate = category.CreatedDate,
                UpdatedDate = category.UpdatedDate
            };
        }
    }
}