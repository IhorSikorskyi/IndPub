using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class BookConfiguration : BaseEntityConfiguration<Book>
{
    public override void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("books");

        base.Configure(builder);

        builder.Property(b => b.Id)
            .HasColumnName("book_id")
            .IsRequired();

        builder.Property(b => b.Title)
            .HasColumnName("title")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(b => b.Description)
            .HasColumnName("description")
            .HasMaxLength(5000);

        builder.Property(b => b.CoverImageUrl)
            .HasColumnName("cover_image_url")
            .HasMaxLength(2048);
        
        builder.Property(b => b.UpdatedDate)
            .HasColumnName("updated_date")
            .IsRequired();

        builder.Property(b => b.Rating)
            .HasColumnName("rating")
            .HasPrecision(3, 2)
            .HasDefaultValue(0);

        builder.Property(b => b.Language)
            .HasColumnName("language")
            .HasConversion<string>()
            .HasDefaultValue(LanguageCode.En)
            .IsRequired();

        builder.Property(b => b.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasDefaultValue(Status.Ongoing)
            .IsRequired();

        builder.HasOne(b => b.Genre)
            .WithMany(g => g.Books)
            .HasForeignKey(b => b.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Subcategory)
            .WithMany(s => s.Books)
            .HasForeignKey(b => b.SubcategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}