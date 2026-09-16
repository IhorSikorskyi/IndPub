using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class ChapterConfiguration : BaseEntityConfiguration<Chapter>
{
    public override void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.ToTable("chapters");

        base.Configure(builder);

        builder.Property(c => c.Id)
            .HasColumnName("chapter_id")
            .IsRequired();

        builder.Property(c => c.Title)
            .HasColumnName("title")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Content)
            .HasColumnName("content")
            .IsRequired();

        builder.Property(c => c.ChapterNumber)
            .HasColumnName("chapter_number")
            .IsRequired();

        builder.HasOne(c => c.Book)
            .WithMany(b => b.Chapters)
            .HasForeignKey(c => c.BookId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}