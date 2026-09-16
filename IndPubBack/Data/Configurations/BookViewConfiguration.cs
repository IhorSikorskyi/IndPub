using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class BookViewConfiguration : IEntityTypeConfiguration<BookView>
{
    public void Configure(EntityTypeBuilder<BookView> builder)
    {
        builder.ToTable("book_views");

        builder.HasKey(bv => new { bv.UserId, bv.BookId })
            .HasName("pk_book_views");

        builder.Property(bv => bv.UserId).HasColumnName("user_id");
        builder.Property(bv => bv.BookId).HasColumnName("book_id");

        builder.HasOne(bv => bv.User)
            .WithMany(u => u.BookViews)
            .HasForeignKey(bv => bv.UserId);

        builder.HasOne(bv => bv.Book)
            .WithMany(b => b.BookViews)
            .HasForeignKey(bv => bv.BookId);

        builder.Property(bv => bv.ViewedAt)
            .HasColumnName("viewed_at")
            .IsRequired();
    }
}