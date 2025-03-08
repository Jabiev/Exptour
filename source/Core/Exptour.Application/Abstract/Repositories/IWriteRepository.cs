namespace Exptour.Application.Abstract.Repositories;

public interface IWriteRepository<T> : IRepository<T> where T : class
{
    Task<T> AddAsync(T entity);
    Task AddRangeAsync(List<T> entities);
    bool Update(T entity);
    void Remove(T entity);
    void RemoveRange(IQueryable<T> entities);
    Task<int> SaveChangesAsync();
}
