using Exptour.Application.Abstract.Repositories;
using Exptour.Domain.Entities.Common;
using Exptour.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Exptour.Persistence.Concrete.Repositories;

public class WriteRepository<T> : IWriteRepository<T> where T : BaseEntity
{
    protected readonly TourismManagementDbContext _tourismManagementDbContext;
    public WriteRepository(TourismManagementDbContext tourismManagementDbContext)
    {
        _tourismManagementDbContext = tourismManagementDbContext;
    }

    public DbSet<T> Table => _tourismManagementDbContext.Set<T>();

    public async Task<int> SaveChangesAsync() => await _tourismManagementDbContext.SaveChangesAsync();

    public async Task AddRangeAsync(List<T> entities) => await Table.AddRangeAsync(entities);

    public async Task<T> AddAsync(T entity)
    {
        await Table.AddAsync(entity);
        return entity;
    }

    public void Remove(T entity) => _tourismManagementDbContext.Remove(entity);

    public void RemoveRange(IQueryable<T> entities) => _tourismManagementDbContext.RemoveRange(entities);

    public bool Update(T entity)
    {
        EntityEntry entry = Table.Update(entity);
        return entry.State == EntityState.Modified;
    }
}
