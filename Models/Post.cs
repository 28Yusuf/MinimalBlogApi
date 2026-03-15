using TechBlogApi.Models.Common;

namespace TechBlogApi.Models
{
    public class Post : BaseEntity
    {
        public string Content { get; set; } = string.Empty;

        public int UserId { get; set; }
        public AppUser? User { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}