using Microsoft.EntityFrameworkCore;

namespace Exptour.Application.Abstract.Repositories;

public interface IRepository<T> where T : class
{
    DbSet<T> Table { get; }
}
