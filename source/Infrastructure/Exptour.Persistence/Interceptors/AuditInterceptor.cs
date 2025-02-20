using Exptour.Common.Helpers;
using Exptour.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Exptour.Persistence.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            UpdateAuditFields(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            UpdateAuditFields(eventData.Context);

        return new ValueTask<InterceptionResult<int>>(result);
    }

    private void UpdateAuditFields(DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedDate = DateTime.UtcNow.ToUAE();

            if (entry.State == EntityState.Modified && entry.Properties.Any(p => p.IsModified))
                entry.Entity.ModifiedDate = DateTime.UtcNow.ToUAE();
        }
    }
}
