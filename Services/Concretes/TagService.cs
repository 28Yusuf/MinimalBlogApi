using Microsoft.EntityFrameworkCore;
using TechBlogApi.Dtos.Tag;
using TechBlogApi.Exceptions;
using TechBlogApi.Helpers;
using TechBlogApi.Mappers;
using TechBlogApi.Models;
using TechBlogApi.Services.Abstracts;
using TechBlogApi.UnitOfWorks;

namespace TechBlogApi.Services.Concretes
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork unitOfWork;

        public TagService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ApiResult> CreateTagAsync(CreateTagDto dto)
        {
            Tag tag = new Tag
            {
                Name = dto.Name
            };
            await unitOfWork.GetWriteRepository<Tag>().AddAsync(tag);
            int result = await unitOfWork.SaveAsync();
            return result > 0 ? new ApiResult(true, "Created Successfully") : new ApiResult(false, "Failed");
        }

        public async Task<ApiResult> DeleteTagAsync(int id)
        {
            await unitOfWork.GetWriteRepository<Tag>().SoftDeleteAsync(id);
            int result = await unitOfWork.SaveAsync();
            return result > 0 ? new ApiResult(true, "Deleted Successfully") : new ApiResult(false, "Failed");
        }

        public async Task<ApiResult<IList<TagDto>>> GetAllTagsAsync()
        {
            IQueryable<Tag> tags = unitOfWork.GetReadRepository<Tag>().GetAllQueryable();
            IQueryable<TagDto> dtos = tags.Select(x => x.ToDto());
            return new ApiResult<IList<TagDto>>(true, await dtos.ToListAsync(), dtos.Count());
        }

        public async Task<ApiResult<TagDto>> GetByIdTagAsync(int id)
        {
            Tag tag = await unitOfWork.GetReadRepository<Tag>().GetAsync(x => x.Id == id);
            if (tag == null) throw new NotFoundException("Tag can't found");
            return new ApiResult<TagDto>(true, tag.ToDto());
        }

        public async Task<ApiResult> UpdateTagAsync(UpdateTagDto dto)
        {
            Tag tag = await unitOfWork.GetReadRepository<Tag>().GetAsync(x => x.Id == dto.Id);
            tag.Name = dto.Name;
            unitOfWork.GetWriteRepository<Tag>().Update(tag);
            int result = await unitOfWork.SaveAsync();
            return result > 0 ? new ApiResult(true, "Updated Successfully") : new ApiResult(false, "Failed");
        }
    }
}