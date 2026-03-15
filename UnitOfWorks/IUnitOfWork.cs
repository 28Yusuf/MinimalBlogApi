using TechBlogApi.Models.Common;
using TechBlogApi.Repositories.Interfaces;

namespace TechBlogApi.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        IReadRepository<T> GetReadRepository<T>() where T : class, IEntity, new();
        IWriteRepository<T> GetWriteRepository<T>() where T : class, IEntity, new();
        Task<int> SaveAsync();
        int Save();
    }
}