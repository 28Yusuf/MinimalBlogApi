using Microsoft.EntityFrameworkCore;
using TechBlogApi.Dtos.Category;
using TechBlogApi.Dtos.Contact;
using TechBlogApi.Dtos.Tag;
using TechBlogApi.Helpers;
using TechBlogApi.Mappers;
using TechBlogApi.Models;
using TechBlogApi.Services.Abstracts;
using TechBlogApi.UnitOfWorks;

namespace TechBlogApi.Services.Concretes
{
    public class ContactService : IContactService
    {
        private readonly IUnitOfWork unitOfWork;

        public ContactService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ApiResult> CreateContactAsync(CreateContactDto dto)
        {
            Contact contact = new Contact
            {
                Name = dto.Name,
                Email = dto.Email,
                Message = dto.Message,
            };
            await unitOfWork.GetWriteRepository<Contact>().AddAsync(contact);
            int result = await unitOfWork.SaveAsync();
            return result > 0 ? new ApiResult(true, "Created Successfully") : new ApiResult(false, "Failed");
        }

        public async Task<ApiResult> DeleteContactAsync(int id)
        {
            await unitOfWork.GetWriteRepository<Contact>().SoftDeleteAsync(id);
            int result = await unitOfWork.SaveAsync();
            return result > 0 ? new ApiResult(true, "Deleted Successfully") : new ApiResult(false, "Failed");
        }

        public async Task<ApiResult<IList<ContactDto>>> GetAllContactAsync()
        {
            var categoryDtos = unitOfWork.GetReadRepository<Contact>().GetAllQueryable().Select(x => x.ToDto());
            return new ApiResult<IList<ContactDto>>(true, categoryDtos.ToList(), await categoryDtos.CountAsync());
        }

        public async Task<ApiResult<ContactDto>> GetAsyncContact(int id)
        {
            Contact contact = await unitOfWork.GetReadRepository<Contact>().GetAsync(x => x.Id == id);
            return new ApiResult<ContactDto>(true, contact.ToDto());
        }

        public async Task<ApiResult> UpdateContactAsync(UpdateContactDto dto)
        {
            Contact contact = await unitOfWork.GetReadRepository<Contact>().GetAsync(x => x.Id == dto.Id);
            contact.Name = dto.Name;
            contact.Email = dto.Email;
            contact.Message = dto.Message;
            unitOfWork.GetWriteRepository<Contact>().Update(contact);
            int result = await unitOfWork.SaveAsync();
            return result > 0 ? new ApiResult(true, "Updated Successfully") : new ApiResult(false, "Failed");
        }
    }
}