using Microsoft.EntityFrameworkCore;
using TechBlogApi.Dtos.SocialLink;
using TechBlogApi.Exceptions;
using TechBlogApi.Helpers;
using TechBlogApi.Mappers;
using TechBlogApi.Models;
using TechBlogApi.Services.Abstracts;
using TechBlogApi.UnitOfWorks;

namespace TechBlogApi.Services.Concretes
{
    public class SocialService : ISocialService
    {
        private readonly IUnitOfWork unitOfWork;

        public SocialService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ApiResult> CreateSocial(CreateSocialDto dto)
        {
            SocialLink socialLink = new()
            {
                Url = dto.Url,
                Platform = dto.Platform,
                UserId = dto.UserId
            };
            await unitOfWork.GetWriteRepository<SocialLink>().AddAsync(socialLink);
            int result = await unitOfWork.SaveAsync();
            return result > 0 ? new ApiResult(true, "Created Successfully") : new ApiResult(false, "Failed");
        }

        public async Task<ApiResult> DeleteSocial(int id)
        {
            await unitOfWork.GetWriteRepository<SocialLink>().SoftDeleteAsync(id);
            int result = await unitOfWork.SaveAsync();
            return result > 0 ? new ApiResult(true, "Deleted Successfully") : new ApiResult(false, "Failed");
        }

        public async Task<ApiResult<IList<SocialDto>>> GetAllSocialsAsync()
        {
            IList<SocialDto> socialLinks = await unitOfWork.GetReadRepository<SocialLink>().GetAllQueryable().Select(x => x.ToDto()).ToListAsync();
            return new ApiResult<IList<SocialDto>>(true, socialLinks, socialLinks.Count);
        }

        public async Task<ApiResult<SocialDto>> GetByIdSocialAsync(int id)
        {
            SocialLink social = await unitOfWork.GetReadRepository<SocialLink>().GetAsync(x => x.Id == id);
            if (social == null) throw new NotFoundException("SocialLinks Not Found");
            return new ApiResult<SocialDto>(true, social.ToDto());
        }

        public async Task<ApiResult> UpdateSocial(UpdateSocialDto dto)
        {
            SocialLink social = await unitOfWork.GetReadRepository<SocialLink>().GetAsync(x => x.Id == dto.Id);
            if (social == null) throw new NotFoundException("SocialLinks Not Found");

            social.Url = dto.Url;
            social.Platform = dto.Platform;

            unitOfWork.GetWriteRepository<SocialLink>().Update(social);
            int result = await unitOfWork.SaveAsync();
            return result > 0 ? new ApiResult(true, "Updated Successfully") : new ApiResult(false, "Failed");
        }
    }
}