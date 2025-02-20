using Exptour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exptour.Persistence.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.HasIndex(l => l.Code)
            .IsUnique();

        builder.HasIndex(l => l.NameEN)
            .IsUnique();

        builder.HasIndex(l => l.NameAR)
            .IsUnique();
    }
}
