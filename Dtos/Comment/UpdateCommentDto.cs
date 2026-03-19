namespace TechBlogApi.Dtos.Comment
{
    public class UpdateCommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int PostId { get; set; }
    }
}