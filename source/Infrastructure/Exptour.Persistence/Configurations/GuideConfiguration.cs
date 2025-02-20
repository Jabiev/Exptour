using Exptour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exptour.Persistence.Configurations;

public class GuideConfiguration : IEntityTypeConfiguration<Guide>
{
    public void Configure(EntityTypeBuilder<Guide> builder)
    {
        builder.HasIndex(g => g.FullName)
            .IsUnique();
    }
}
