using TechBlogApi.Models.Common;

namespace TechBlogApi.Models
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }
}