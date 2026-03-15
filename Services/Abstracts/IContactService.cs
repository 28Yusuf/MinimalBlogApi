using TechBlogApi.Dtos.Contact;
using TechBlogApi.Helpers;

namespace TechBlogApi.Services.Abstracts
{
    public interface IContactService
    {
        Task<ApiResult<IEnumerable<ContactDto>>> GetAllContactAsync();
        Task<ApiResult<ContactDto>> GetAsyncContact(int id);
        Task<ApiResult> CreateContactAsync(CreateContactDto dto);
        Task<ApiResult> UpdateContactAsync(UpdateContactDto dto);
        Task<ApiResult> DeleteContactAsync(int id);
    }
}