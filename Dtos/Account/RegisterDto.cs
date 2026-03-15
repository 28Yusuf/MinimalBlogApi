namespace TechBlogApi.Dtos.Account
{
    public class RegisterDto
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Bio { get; set; } = "";
        public string Password { get; set; } = "";
    }
}