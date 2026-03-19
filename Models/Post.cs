using TechBlogApi.Models.Common;

namespace TechBlogApi.Models
{
    public class Post : BaseEntity
    {
        public string Content { get; set; } = string.Empty;

        public int UserId { get; set; }
        public AppUser? User { get; set; }

        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public int PostBookMarkId { get; set; }
        public virtual PostBookMark? PostBookMark { get; set; }
        
        public int PostLikeId { get; set; }
        public virtual PostLike? PostLike { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Tag>? Tags { get; set; }
    }
}