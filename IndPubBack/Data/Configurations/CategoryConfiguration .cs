using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class CategoryConfiguration : BaseEntityConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        base.Configure(builder);

        builder.Property(c =>c.Id)
            .HasColumnName("category_id")
            .IsRequired();

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.HasMany(c => c.Books)
            .WithOne(b => b.Category)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}