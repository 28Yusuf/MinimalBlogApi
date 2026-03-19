using Microsoft.EntityFrameworkCore;
using TechBlogApi.Dtos.Category;
using TechBlogApi.Helpers;
using TechBlogApi.Mappers;
using TechBlogApi.Models;
using TechBlogApi.Services.Abstracts;
using TechBlogApi.UnitOfWorks;

namespace TechBlogApi.Services.Concretes
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult> CreateCategoryAsync(CreateCategoryDto dto)
        {
            Category category = new()
            {
                Name = dto.Name,
            };
            await _unitOfWork.GetWriteRepository<Category>().AddAsync(category);
            int result = await _unitOfWork.SaveAsync();
            return result > 0 ? new ApiResult(true, "Categroy Created Successfully") : new ApiResult(false, "It cant be Created");
        }

        public async Task<ApiResult> DeleteCategoryAsync(int id)
        {
            Category category = await _unitOfWork.GetReadRepository<Category>().GetAsync(x => x.Id == id);
            _unitOfWork.GetWriteRepository<Category>().Remove(category);
            int result = await _unitOfWork.SaveAsync();
            return result > 0 ? new ApiResult(true, "Category Deleted Succcessfully") : new ApiResult(false, "It cant be deleted");
        }

        public async Task<ApiResult<IList<CategoryDto>>> GetAllCategoryAsync()
        {
            var categories = _unitOfWork.GetReadRepository<Category>().GetAllQueryable();
            IList<CategoryDto> dtos = await categories.Select(x => new CategoryDto()
            {
                Id = x.Id,
                Name = x.Name,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate
            }).ToListAsync();
            return new ApiResult<IList<CategoryDto>>(true, dtos, categories.Count());
        }

        public async Task<ApiResult<CategoryDto>> GetAsyncCategory(int id)
        {
            Category category = await _unitOfWork.GetReadRepository<Category>().GetAsync(x => x.Id == id);
            return new ApiResult<CategoryDto>(true, category.ToDto());
        }

        public async Task<ApiResult> UpdateCategoryAsync(UpdateCategoryDto dto)
        {
            Category category = await _unitOfWork.GetReadRepository<Category>().GetAsync(x => x.Id == dto.Id);
            category.Name = dto.Name;
            _unitOfWork.GetWriteRepository<Category>().Update(category);
            int result = await _unitOfWork.SaveAsync();
            return result > 0 ? new ApiResult(true, "Category Updated Succcessfully") : new ApiResult(false, "It cant be updated");
        }
    }
}