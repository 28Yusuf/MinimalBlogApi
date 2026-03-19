using Carter;
using Microsoft.AspNetCore.Mvc;
using TechBlogApi.Dtos.Post;
using TechBlogApi.Helpers;
using TechBlogApi.Services.Abstracts;

namespace TechBlogApi.Endpoints
{
    public class PostModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/post")
            .RequireAuthorization()
            .WithTags("Post");

            group.MapGet("", async ([FromServices] IPostService service,
                [FromQuery] string? searchTerm,
                [FromQuery] int pageNumber,
                [FromQuery] int pageSize,
                [FromQuery] string? sortBy,
                [FromQuery] bool isDescending) =>
            {
                QueryObject query = new QueryObject
                {
                    SearchTerm = searchTerm,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SortBy = sortBy,
                    IsDescending = isDescending
                };

                return Results.Ok(await service.GetAllPostsAsync(query));
            });

            group.MapPost("", async ([FromServices] IPostService service, CreatePostDto dto) =>
            {
                return Results.Ok(await service.CreatePost(dto));
            });
        }
    }
}