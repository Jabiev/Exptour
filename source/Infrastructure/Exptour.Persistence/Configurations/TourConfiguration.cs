using Exptour.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exptour.Persistence.Configurations;

public class TourConfiguration : IEntityTypeConfiguration<Tour>
{
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.OwnsOne(t => t.MeetingPoint, builder =>
        {
            builder.Property(mp => mp.Latitude)
                .HasColumnName("MeetingLatitude")
                .HasPrecision(9, 6);
            builder.Property(mp => mp.Longitude)
                .HasColumnName("MeetingLongitude")
                .HasPrecision(9, 6);
        });

        builder.OwnsOne(t => t.EndPoint, builder =>
        {
            builder.Property(mp => mp.Latitude)
                .HasColumnName("EndLatitude")
                .HasPrecision(9, 6);
            builder.Property(mp => mp.Longitude)
                .HasColumnName("EndLongitude")
                .HasPrecision(9, 6);
        });
    }
}
