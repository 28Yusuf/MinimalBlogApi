using Carter;
using Microsoft.AspNetCore.Mvc;
using TechBlogApi.Filters;
using TechBlogApi.Services.Abstracts;

namespace TechBlogApi.Endpoints
{
    public class NewsLetterModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/newsletter")
                .WithTags("NewsLetter");

            group.MapPost("/subscribe", async (INewsLetterService service,
                [FromBody] string email) =>
            {
                return Results.Ok(await service.SubscribeAsync(email));
            }).WithName("SubscribeToNewsletter")
              .WithDescription("Subscribe to newsletter with email address")
              .AllowAnonymous(); 

            group.MapPost("/unsubscribe", async ([FromServices] INewsLetterService service,
                [FromBody] string email) =>
            {
                return Results.Ok(await service.UnsubscribeAsync(email));
            }).WithName("UnsubscribeFromNewsletter")
              .WithDescription("Unsubscribe from newsletter")
              .AllowAnonymous();

            group.MapGet("/check/{email}", async ([FromServices] INewsLetterService service,
                string email) =>
            {
                return Results.Ok(await service.IsSubscribedAsync(email));
            }).WithName("CheckSubscription")
              .WithDescription("Check if email is subscribed to newsletter")
              .AllowAnonymous();

            group.MapGet("/subscribers", async ([FromServices] INewsLetterService service) =>
            {
                return Results.Ok(await service.GetAllSubscribersAsync());
            }).WithName("GetAllSubscribers")
              .WithDescription("Get all newsletter subscribers")
              .AddEndpointFilter(new AdminOnlyPostFilter()); 
        }
    }
}