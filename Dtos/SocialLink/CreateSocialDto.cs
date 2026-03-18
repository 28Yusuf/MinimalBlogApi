namespace TechBlogApi.Dtos.SocialLink
{
    public class CreateSocialDto
    {
        public string Platform { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}