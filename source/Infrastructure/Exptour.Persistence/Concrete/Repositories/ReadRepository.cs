using Exptour.Application.Abstract.Repositories;
using Exptour.Domain.Entities.Common;
using Exptour.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;

namespace Exptour.Persistence.Concrete.Repositories;

public class ReadRepository<T> : IReadRepository<T> where T : BaseEntity
{
    protected readonly TourismManagementDbContext _tourismManagementDbContext;
    public ReadRepository(TourismManagementDbContext tourismManagementDbContext)
    {
        _tourismManagementDbContext = tourismManagementDbContext;
    }

    public DbSet<T> Table => _tourismManagementDbContext.Set<T>();

    public IQueryable<T> GetAll() => Table.AsNoTracking();

    public IQueryable<T> GetAll(Expression<Func<T, bool>> expression,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        Expression<Func<T, object?>>? orderBy = null,
        bool ascending = true)
    {
        IQueryable<T> query = Table;

        if (include is not null)
        {
            query = include(query);
        }

        if (expression is not null)
            query = query.Where(expression);

        if (orderBy is not null)
            query = ascending
                ? query.OrderBy(orderBy)
                : query.OrderByDescending(orderBy);

        return query;
    }

    public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        => Table.Where(expression);

    public IQueryable<T> WhereAsNoTracking(Expression<Func<T, bool>> expression)
        => Table.AsNoTracking().Where(expression);

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression, bool tracking = true)
        => await (tracking ? Table : Table.AsNoTracking())
        .FirstOrDefaultAsync(expression);

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression, Func<IQueryable<T>, IQueryable<T>> include)
    {
        IQueryable<T> query = Table;

        if (include is not null)
            query = include(query);

        return await query.FirstOrDefaultAsync(expression);
    }

    public async Task<T?> GetByIdAsync(Guid id, bool tracking = true)
        => await (tracking ? Table : Table.AsNoTracking())
        .FirstOrDefaultAsync(data => data.Id == id);

    public async Task<T?> FindByIdAsync(Guid id)
        => await Table.FindAsync(id);

    public async Task<T?> GetByFiltered(Expression<Func<T, bool>> expression)
        => await Table.Where(expression).AsNoTracking().FirstOrDefaultAsync();
}
