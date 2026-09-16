using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class CommentConfiguration : BaseEntityConfiguration<Comment>
{
    public override void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("comments", t => t.HasCheckConstraint(
            "ck_comments_chapter_xor_review",
            "(chapter_id IS NOT NULL AND review_id IS NULL) OR (chapter_id IS NULL AND review_id IS NOT NULL)"));

        base.Configure(builder);

        builder.Property(c => c.Id)
            .HasColumnName("comment_id")
            .IsRequired();

        builder.Property(c => c.Text)
            .HasColumnName("text")
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.ChapterId)
            .HasColumnName("chapter_id");

        builder.Property(c => c.ReviewId)
            .HasColumnName("review_id");

        builder.HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(c => c.Chapter)
            .WithMany(ch => ch.Comments)
            .HasForeignKey(c => c.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Review)
            .WithMany(r => r.Comments)
            .HasForeignKey(c => c.ReviewId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}