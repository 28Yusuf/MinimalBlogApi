using Microsoft.AspNetCore.Identity;

namespace TechBlogApi.Models
{
    public class AppUser : IdentityUser<int>
    {
        public string Bio { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenEndDate { get; set; }
    }
}