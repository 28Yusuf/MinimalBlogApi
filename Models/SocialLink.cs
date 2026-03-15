using TechBlogApi.Models.Common;

namespace TechBlogApi.Models
{
    public class SocialLink : BaseEntity
    {
        public string Platform { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int UserId { get; set; }
        public AppUser? User { get; set; }
    }
}