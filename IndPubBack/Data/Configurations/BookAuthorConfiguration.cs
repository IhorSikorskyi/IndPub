using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class BookAuthorConfiguration : IEntityTypeConfiguration<BookAuthor>
{
    public void Configure(EntityTypeBuilder<BookAuthor> builder)
    {
        builder.ToTable("book_authors");

        builder.HasKey(ba => new { ba.BookId, ba.UserId })
            .HasName("pk_book_authors");

        builder.Property(ba => ba.UserId)
            .HasColumnName("user_id");

        builder.Property(ba => ba.BookId)
            .HasColumnName("book_id");

        builder.HasOne(ba => ba.Book)
            .WithMany(b => b.BookAuthors)
            .HasForeignKey(ba => ba.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ba => ba.User)
            .WithMany(u => u.BookAuthors)
            .HasForeignKey(ba => ba.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}