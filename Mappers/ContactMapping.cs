using TechBlogApi.Dtos.Contact;
using TechBlogApi.Models;

namespace TechBlogApi.Mappers
{
    public static class ContactMapping
    {
        public static ContactDto ToDto(this Contact contact)
        {
            return new ContactDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Message = contact.Message,
                CreatedBy = contact.CreatedBy,
                CreatedDate = contact.CreatedDate,
                UpdatedBy = contact.UpdatedBy,
                UpdatedDate = contact.UpdatedDate
            };
        }
    }
}