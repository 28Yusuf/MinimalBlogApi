using TechBlogApi.Models.Common;

namespace TechBlogApi.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }
}