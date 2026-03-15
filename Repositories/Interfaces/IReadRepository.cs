using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using TechBlogApi.Models;
using TechBlogApi.Models.Common;

namespace TechBlogApi.Repositories.Interfaces
{
    public interface IReadRepository<T> where T : class,IEntity, new()
    {
        IQueryable<T> GetAllQueryable();
        Task<T> GetAsync(Expression<Func<T, bool>> expression);
        IQueryable<T> Find(Expression<Func<T, bool>> predicate, bool enableTracking = false);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<IList<T>> GetAllByPagingAsync(Expression<Func<T, bool>>? predicate = null,
    Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
    bool enableTracking = false, int currentPage = 1, int pageSize = 3);
    }
}