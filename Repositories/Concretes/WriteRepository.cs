using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TechBlogApi.Context;
using TechBlogApi.Models;
using TechBlogApi.Models.Common;
using TechBlogApi.Repositories.Interfaces;

namespace TechBlogApi.Repositories.Concretes
{
    public class WriteRepository<T> : IWriteRepository<T> where T : class, IEntity, new()
    {
        private readonly AppDbContext _context;
        public WriteRepository(AppDbContext context)
        {
            _context = context;
        }

        private DbSet<T> Table { get => _context.Set<T>(); }

        public async Task AddAsync(T entity)
        {
            EntityEntry<T> entityEntry = await Table.AddAsync(entity);
        }

        public void Remove(T entity)
        {
            EntityEntry<T> entityEntry = Table.Remove(entity);
        }

        public async Task SoftDeleteAsync(int id)
        {
            var entity = await Table.FindAsync(id);
            if (entity is not null && entity is BaseEntity baseEntity)
            {
                baseEntity.IsDeleted = true;
                EntityEntry<T> entityEntry = Table.Update(entity);
            }
        }

        public void Update(T entity)
        {
            EntityEntry<T> entityEntry = Table.Update(entity);
        }
    }
}