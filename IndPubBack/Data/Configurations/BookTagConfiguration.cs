using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class BookTagConfiguration : IEntityTypeConfiguration<BookTag>
{
    public void Configure(EntityTypeBuilder<BookTag> builder)
    {
        builder.ToTable("book_tags");

        builder.HasKey(bt => new { bt.BookId, bt.TagId })
            .HasName("pk_book_tags");

        builder.Property(bt => bt.BookId)
            .HasColumnName("book_id");

        builder.Property(bt => bt.TagId)
            .HasColumnName("tag_id");

        builder.HasOne(bt => bt.Book)
            .WithMany(b => b.BookTags)
            .HasForeignKey(bt => bt.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bt => bt.Tag)
            .WithMany(t => t.BookTags)
            .HasForeignKey(bt => bt.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}