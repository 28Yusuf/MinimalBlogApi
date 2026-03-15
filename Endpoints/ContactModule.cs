using Carter;
using TechBlogApi.Dtos.Contact;
using TechBlogApi.Services.Abstracts;

namespace TechBlogApi.Endpoints
{
    public class ContactModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/contact")
            .WithTags("Contact")
            .RequireAuthorization();

            group.MapGet("", async (IContactService contactService) =>
            {
                return Results.Ok(await contactService.GetAllContactAsync());
            });

            group.MapGet("/{id}", async (int id, IContactService service) =>
            {
                return Results.Ok(await service.GetAsyncContact(id));
            });

            group.MapPost("", async (CreateContactDto dto, IContactService service) =>
            {
                return Results.Ok(await service.CreateContactAsync(dto));
            });

            group.MapPut("", async (UpdateContactDto dto, IContactService service) =>
            {
                return Results.Ok(await service.UpdateContactAsync(dto));
            });

            group.MapDelete("/{id}", async (int id, IContactService service) =>
            {
              return Results.Ok(await service.DeleteContactAsync(id));
            });
        }
    }
}