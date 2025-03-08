using Exptour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exptour.Persistence.Configurations;

public class EndpointRoleConfiguration : IEntityTypeConfiguration<EndpointRole>
{
    public void Configure(EntityTypeBuilder<EndpointRole> builder)
    {
        builder.HasKey(er => new { er.EndpointId, er.RoleId });

        builder.HasOne(er => er.Endpoint)
            .WithMany(e => e.EndpointRoles)
            .HasForeignKey(er => er.EndpointId);

        builder.HasOne(er => er.Role)
            .WithMany()
            .HasForeignKey(er => er.RoleId);
    }
}
