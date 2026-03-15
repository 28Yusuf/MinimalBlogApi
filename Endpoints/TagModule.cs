using Carter;
using TechBlogApi.Dtos.Tag;
using TechBlogApi.Services.Abstracts;

namespace TechBlogApi.Endpoints
{
    public class TagModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/tag")
            .RequireRateLimiting("fixed")
            .RequireAuthorization();

            group.MapGet(string.Empty, async (ITagService service) =>
            {
                return Results.Ok(await service.GetAllTagsAsync());
            }).Produces<TagDto>();

            group.MapGet("/{id}", async (ITagService service, int id) =>
            {
                return Results.Ok(await service.GetByIdTagAsync(id));
            }).Produces<TagDto>();

            group.MapPost(string.Empty, async (ITagService service, CreateTagDto dto) =>
            {
                return Results.Ok(await service.CreateTagAsync(dto));
            });

            group.MapPut(string.Empty, async (ITagService service, UpdateTagDto dto) =>
            {
                return Results.Ok(await service.UpdateTagAsync(dto));
            });

            group.MapDelete(string.Empty, async (ITagService service, int id) =>
            {
                return Results.Ok(await service.DeleteTagAsync(id));
            });
        }
    }
}