namespace TechBlogApi.Models
{
    public class TokenModel
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpirationTime { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}