using TechBlogApi.Models;
using TechBlogApi.Models.Common;

namespace TechBlogApi.Repositories.Interfaces
{
    public interface IWriteRepository<T> where T : class, IEntity, new()
    {
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task SoftDeleteAsync(int id);
    }
}