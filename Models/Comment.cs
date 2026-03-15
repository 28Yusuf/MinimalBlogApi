using TechBlogApi.Models.Common;

namespace TechBlogApi.Models
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;

        public int UserId { get; set; }
        public AppUser? User { get; set; }

        public int PostId { get; set; }
        public Post? Post { get; set; }
    }
}