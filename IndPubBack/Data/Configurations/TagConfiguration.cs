using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class TagConfiguration : BaseEntityConfiguration<Tag>
{
    public override void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");

        base.Configure(builder);

        builder.HasIndex(t => t.Name)
            .IsUnique()
            .HasDatabaseName("ix_tags_name");
    }
}