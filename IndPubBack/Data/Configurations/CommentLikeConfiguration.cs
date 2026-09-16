using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class CommentLikeConfiguration : IEntityTypeConfiguration<CommentLike>
{
    public void Configure(EntityTypeBuilder<CommentLike> builder)
    {
        builder.ToTable("comment_likes");

        builder.HasKey(cl => new { cl.CommentId, cl.UserId })
            .HasName("pk_comment_likes");

        builder.Property(cl => cl.CommentId)
            .HasColumnName("comment_id")
            .IsRequired();

        builder.Property(cl => cl.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.HasOne(cl => cl.Comment)
            .WithMany(c => c.CommentLikes)
            .HasForeignKey(cl => cl.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cl => cl.User)
            .WithMany(u => u.CommentLikes)
            .HasForeignKey(cl => cl.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}