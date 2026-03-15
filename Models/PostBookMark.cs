using TechBlogApi.Models.Common;

namespace TechBlogApi.Models
{
    public class PostBookMark : BaseEntity
    {
        public int UserId { get; set; }
        public AppUser? User { get; set; }
        public int PostId { get; set; }
        public Post? Post { get; set; }
    }
}