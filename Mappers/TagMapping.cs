using TechBlogApi.Dtos.Tag;
using TechBlogApi.Models;

namespace TechBlogApi.Mappers
{
    public static class TagMapping
    {
        public static TagDto ToDto(this Tag tag)
        {
            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                CreatedBy = tag.CreatedBy,
                CreatedDate = tag.CreatedDate,
                UpdatedBy = tag.UpdatedBy,
                UpdatedDate = tag.UpdatedDate
            };
        }
    }
}