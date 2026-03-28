using System.Security.Claims;
using Carter;
using Microsoft.AspNetCore.Mvc;
using TechBlogApi.Dtos.Comment;
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
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 10,
                [FromQuery] string? sortBy = "",
                [FromQuery] bool isDescending = false) =>
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
            }).WithName("GetAllPosts")
              .WithDescription("Get all posts with pagination, search, and sorting");

            group.MapGet("/{id}", async ([FromServices] IPostService service, int id) =>
            {
                return Results.Ok(await service.GetByIdPostAsync(id));
            }).WithName("GetPostById")
              .WithDescription("Get a specific post by ID");


            group.MapPost("", async ([FromServices] IPostService service, CreatePostDto dto) =>
            {
                return Results.Ok(await service.CreatePost(dto));
            }).WithName("CreatePost")
              .WithDescription("Create a new post");


            group.MapPut("/{id}", async ([FromServices] IPostService service, int id, UpdatePostDto dto) =>
            {
                return Results.Ok(await service.UpdatePost(dto));
            }).WithName("UpdatePost")
              .WithDescription("Update an existing post");

            group.MapDelete("/{id}", async ([FromServices] IPostService service, int id) =>
            {
                return Results.Ok(await service.DeletePost(id));
            }).WithName("DeletePost")
              .WithDescription("Soft delete a post");

            group.MapGet("/category/{categoryId}", async ([FromServices] IPostService service, int categoryId) =>
            {
                return Results.Ok(await service.GetPostsByCategoryAsync(categoryId));
            }).WithName("GetPostsByCategory")
              .WithDescription("Get all posts by category ID");

            group.MapGet("/user/{userId}", async ([FromServices] IPostService service, int userId) =>
            {
                return Results.Ok(await service.GetPostsByUserAsync(userId));
            }).WithName("GetPostsByUser")
              .WithDescription("Get all posts by user ID");

            group.MapGet("/{postId}/comments", async ([FromServices] IPostService service, int postId) =>
            {
                return Results.Ok(await service.GetCommentsByPostAsync(postId));
            }).WithName("GetCommentsByPost")
              .WithDescription("Get all comments for a specific post");

            group.MapPost("/{postId}/comments", async ([FromServices] IPostService service, int postId, CreateCommentDto dto) =>
            {
                return Results.Ok(await service.AddCommentToPost(dto));
            }).WithName("AddCommentToPost")
              .WithDescription("Add a comment to a post");

            group.MapPut("/comments/{commentId}", async ([FromServices] IPostService service, int commentId, UpdateCommentDto dto) =>
            {
                return Results.Ok(await service.UpdateComment(dto));
            }).WithName("UpdateComment")
              .WithDescription("Update a comment");

            group.MapDelete("/comments/{commentId}", async ([FromServices] IPostService service, int commentId) =>
            {
                return Results.Ok(await service.DeleteComment(commentId));
            }).WithName("DeleteComment")
              .WithDescription("Delete a comment");

            group.MapGet("/{postId}/tags", async ([FromServices] IPostService service, int postId) =>
            {
                return Results.Ok(await service.GetTagsByPostAsync(postId));
            }).WithName("GetTagsByPost")
              .WithDescription("Get all tags for a specific post");

            group.MapPost("/{postId}/tags/{tagId}", async ([FromServices] IPostService service, int postId, int tagId) =>
            {
                return Results.Ok(await service.AddTagToPost(postId, tagId));
            }).WithName("AddTagToPost")
              .WithDescription("Add a tag to a post");

            group.MapDelete("/{postId}/tags/{tagId}", async ([FromServices] IPostService service, int postId, int tagId) =>
            {
                return Results.Ok(await service.RemoveTagFromPost(postId, tagId));
            }).WithName("RemoveTagFromPost")
              .WithDescription("Remove a tag from a post");

            group.MapPost("/{postId}/like", async ([FromServices] IPostService service,
                [FromServices] IHttpContextAccessor contextAccessor, int postId) =>
            {
                var userId = int.Parse(contextAccessor.HttpContext?.User.FindFirstValue("userId") ?? "0");
                return Results.Ok(await service.LikePost(postId, userId));
            }).WithName("LikePost")
              .WithDescription("Like a post");

            group.MapDelete("/{postId}/like", async ([FromServices] IPostService service,
                [FromServices] IHttpContextAccessor contextAccessor, int postId) =>
            {
                var userId = int.Parse(contextAccessor.HttpContext?.User.FindFirstValue("userId") ?? "0");
                return Results.Ok(await service.UnlikePost(postId, userId));
            }).WithName("UnlikePost")
              .WithDescription("Unlike a post");

            group.MapPost("/{postId}/bookmark", async ([FromServices] IPostService service,
                [FromServices] IHttpContextAccessor contextAccessor, int postId) =>
            {
                var userId = int.Parse(contextAccessor.HttpContext?.User.FindFirstValue("userId") ?? "0");
                return Results.Ok(await service.BookmarkPost(postId, userId));
            }).WithName("BookmarkPost")
              .WithDescription("Bookmark a post");

            group.MapDelete("/{postId}/bookmark", async ([FromServices] IPostService service,
                [FromServices] IHttpContextAccessor contextAccessor, int postId) =>
            {
                var userId = int.Parse(contextAccessor.HttpContext?.User.FindFirstValue("userId") ?? "0");
                return Results.Ok(await service.RemoveBookmark(postId, userId));
            }).WithName("RemoveBookmark")
              .WithDescription("Remove bookmark from a post");
        }
    }
}