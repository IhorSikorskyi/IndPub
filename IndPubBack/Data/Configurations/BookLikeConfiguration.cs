using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class BookLikeConfiguration : IEntityTypeConfiguration<BookLike>
{
    public void Configure(EntityTypeBuilder<BookLike> builder)
    {
        builder.ToTable("book_likes");

        builder.HasKey(bl => new { bl.BookId, bl.UserId })
            .HasName("pk_book_likes");

        builder.Property(bl => bl.UserId)
            .HasColumnName("user_id");

        builder.Property(bl => bl.BookId)
            .HasColumnName("book_id");

        builder.Property(bl => bl.LikedAt)
            .HasColumnName("liked_at")
            .IsRequired();

        builder.HasOne(bl => bl.Book)
            .WithMany(b => b.BookLikes)
            .HasForeignKey(bl => bl.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bl => bl.User)
            .WithMany(u => u.BookLikes)
            .HasForeignKey(bl => bl.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}