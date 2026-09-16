using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class GenreConfiguration : BaseEntityConfiguration<Genre>
{
    public override void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("genres");

        base.Configure(builder);

        builder.Property(g => g.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Description)
            .HasColumnName("description")
            .IsRequired()
            .HasMaxLength(500);

        // Relationship to Book is fully configured in BookConfiguration.
        // Don't redeclare it here with a different OnDelete value.
    }
}