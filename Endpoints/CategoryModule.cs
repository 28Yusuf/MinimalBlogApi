using Carter;
using TechBlogApi.Dtos.Category;
using TechBlogApi.Services.Abstracts;

namespace TechBlogApi.Endpoints
{
    public class CategoryModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/category")
            .RequireAuthorization()
            .WithTags("Category");

            group.MapGet(string.Empty, async (ICategoryService service) =>
            {
                return Results.Ok(await service.GetAllCategoryAsync());
            })
            .WithDescription("GetAllCategories");

            group.MapGet("/{id}", async (ICategoryService service, int id) =>
            {
                return Results.Ok(await service.GetAsyncCategory(id));
            });

            group.MapPost(string.Empty, async (ICategoryService service, CreateCategoryDto dto) =>
            {
                return Results.Ok(await service.CreateCategoryAsync(dto));
            });

            group.MapPut(string.Empty, async (ICategoryService service, UpdateCategoryDto dto) =>
            {
                return Results.Ok(await service.UpdateCategoryAsync(dto));
            });
        }
    }
}