using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class LibraryEntryConfiguration : IEntityTypeConfiguration<LibraryEntry>
{
    public void Configure(EntityTypeBuilder<LibraryEntry> builder)
    {
        builder.ToTable("library_entries");

        builder.HasKey(le => new { le.UserId, le.BookId })
            .HasName("pk_library_entries");

        builder.Property(le => le.UserId)
            .HasColumnName("user_id");

        builder.Property(le => le.BookId)
            .HasColumnName("book_id");

        builder.Property(le => le.DateAdded)
            .HasColumnName("date_added")
            .IsRequired();

        builder.Property(le => le.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasDefaultValue(LibraryBookStatus.Reading)
            .IsRequired();

        builder.HasOne(le => le.User)
            .WithMany(u => u.LibraryEntries)
            .HasForeignKey(le => le.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(le => le.Book)
            .WithMany(b => b.LibraryEntries)
            .HasForeignKey(le => le.BookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}