using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TechBlogApi.Context;
using TechBlogApi.Models.Common;
using TechBlogApi.Repositories.Interfaces;

namespace TechBlogApi.Repositories.Concretes
{
    public class ReadRepository<T> : IReadRepository<T> where T : class, IEntity, new()
    {
        private readonly AppDbContext _context;
        public ReadRepository(AppDbContext context)
        {
            _context = context;
        }

        private DbSet<T> Table => _context.Set<T>();

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            IQueryable<T> query = Table;
            if (predicate is not null) query.Where(predicate);
            return await query.CountAsync();
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate, bool enableTracking = false)
        {
            IQueryable<T> query = Table;
            if (enableTracking) query = query.AsNoTracking();
            return query.Where(predicate).AsNoTracking();
        }

        public async Task<IList<T>> GetAllByPagingAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool enableTracking = false, int currentPage = 1, int pageSize = 3)
        {
            IQueryable<T> query = Table;
            if (predicate is not null) query = query.Where(predicate);
            if (include is not null) query = include(query);
            if (orderBy is not null) query = orderBy(query);
            if (enableTracking) query = query.AsNoTracking();
            return await query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public IQueryable<T> GetAllQueryable()
        {

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, "IsDeleted");

            var condition = Expression.NotEqual(property, Expression.Constant(true));

            var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);

            return Table.AsNoTracking().Where(lambda);
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> expression)
        {
            return await Table.Where(expression).FirstAsync();
        }
    }
}