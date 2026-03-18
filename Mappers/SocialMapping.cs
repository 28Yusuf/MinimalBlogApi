using TechBlogApi.Dtos.SocialLink;
using TechBlogApi.Models;

namespace TechBlogApi.Mappers
{
    public static class SocialMapping
    {
        public static SocialDto ToDto(this SocialLink social)
        {
            return new SocialDto
            {
                Id = social.Id,
                Url = social.Url,
                Platform = social.Platform,
                UserId = social.UserId,
                CreatedBy = social.CreatedBy,
                CreatedDate = social.CreatedDate,
                UpdatedBy = social.UpdatedBy,
                UpdatedDate = social.UpdatedDate
            };
        }
    }
}