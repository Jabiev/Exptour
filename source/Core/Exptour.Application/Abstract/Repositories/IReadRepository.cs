using System.Linq.Expressions;

namespace Exptour.Application.Abstract.Repositories;

public interface IReadRepository<T> : IRepository<T> where T : class
{
    IQueryable<T> GetAll();
    IQueryable<T> GetAll(Expression<Func<T, bool>> expression,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        Expression<Func<T, object?>>? orderBy = null,
        bool ascending = true);
    IQueryable<T> Where(Expression<Func<T, bool>> func);
    IQueryable<T> WhereAsNoTracking(Expression<Func<T, bool>> expression);
    Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression, bool tracking = true);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression, Func<IQueryable<T>, IQueryable<T>> include);
    Task<T?> GetByFiltered(Expression<Func<T, bool>> expression);
    Task<T> GetByIdAsync(Guid id, bool tracking = true);
    Task<T> FindByIdAsync(Guid id);
}
