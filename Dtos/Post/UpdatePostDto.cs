namespace TechBlogApi.Dtos.Post
{
    public class UpdatePostDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int CategoryId { get; set; }
    }
}