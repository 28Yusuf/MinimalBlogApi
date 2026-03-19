namespace TechBlogApi.Dtos.Post
{
    public class CreatePostDto
    {
        public string Content { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public List<int>? TagIds { get; set; }
    }
}