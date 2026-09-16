using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
{
    public void Configure(EntityTypeBuilder<Bookmark> builder)
    {
        builder.ToTable("bookmarks");

        builder.HasKey(b => new { b.ChapterId, b.UserId })
            .HasName("pk_bookmarks");

        builder.Property(b => b.UserId)
            .HasColumnName("user_id");

        builder.Property(b => b.ChapterId)
            .HasColumnName("chapter_id");

        builder.HasOne(b => b.Chapter)
            .WithMany(c => c.Bookmarks)
            .HasForeignKey(b => b.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.User)
            .WithMany(u => u.Bookmarks)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}