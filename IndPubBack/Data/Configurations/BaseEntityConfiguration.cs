using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id)
            .HasName("pk_" + builder.Metadata.GetTableName() + "_id");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");
    }
}