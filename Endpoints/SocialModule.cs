using Carter;
using TechBlogApi.Dtos.SocialLink;
using TechBlogApi.Services.Abstracts;

namespace TechBlogApi.Endpoints
{
    public class SocialModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/social")
            .RequireAuthorization()
            .RequireRateLimiting("fixed")
            .WithTags("Social");

            group.MapGet("", async (ISocialService service) =>
            {
                return Results.Ok(await service.GetAllSocialsAsync());
            });

            group.MapGet("/{id}", async (ISocialService service, int id) =>
            {
                return Results.Ok(await service.GetByIdSocialAsync(id));
            });

            group.MapPost("", async (ISocialService service, CreateSocialDto dto) =>
            {
                return Results.Ok(await service.CreateSocial(dto));
            });

            group.MapPut("", async (ISocialService service, UpdateSocialDto dto) =>
            {
                return Results.Ok(await service.UpdateSocial(dto));
            });

            group.MapDelete("/{id}", async (ISocialService service, int id) =>
            {
                return Results.Ok(await service.DeleteSocial(id));
            });
        }
    }
}