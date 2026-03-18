namespace TechBlogApi.Dtos.SocialLink
{
    public class UpdateSocialDto
    {
        public int Id { get; set; }
        public string Platform { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}