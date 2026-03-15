using TechBlogApi.Models.Common;

namespace TechBlogApi.Models
{
    public class NewsLetter : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
    }
}