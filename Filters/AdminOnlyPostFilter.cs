using System.Security.Claims;

namespace TechBlogApi.Filters
{
    public class AdminOnlyPostFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            string? role = context.HttpContext.User.FindFirstValue("role");
            if (role != "admin")
                return Results.Forbid();

            return await next(context);
        }
    }
}