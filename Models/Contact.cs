using TechBlogApi.Models.Common;

namespace TechBlogApi.Models
{
    public class Contact : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}