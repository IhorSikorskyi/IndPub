using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class SubcategoryConfiguration : BaseEntityConfiguration<Subcategory>
{
    public override void Configure(EntityTypeBuilder<Subcategory> builder)
    {
        builder.ToTable("subcategories");

        base.Configure(builder);

        builder.Property(s => s.Id)
            .HasColumnName("subcategory_id")
            .IsRequired();

        builder.Property(s => s.Name)
            .HasColumnName("subcategory_name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Description)
            .HasColumnName("subcategory_description")
            .HasMaxLength(500);
        
        builder.HasMany(s => s.Books)
            .WithOne(b => b.Subcategory)
            .HasForeignKey(b => b.SubcategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Category)
            .WithMany(c => c.Subcategories)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_subcategories_category_id");
    }
}