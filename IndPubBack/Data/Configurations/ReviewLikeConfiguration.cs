using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class ReviewLikeConfiguration : IEntityTypeConfiguration<ReviewLike>
{
    public void Configure(EntityTypeBuilder<ReviewLike> builder)
    {
        builder.ToTable("review_likes");

        builder.HasKey(rl => new { rl.ReviewId, rl.UserId })
            .HasName("pk_review_likes");

        builder.Property(rl => rl.UserId)
            .HasColumnName("user_id");

        builder.Property(rl => rl.ReviewId)
            .HasColumnName("review_id");

        builder.Property(rl => rl.LikedAt)
            .HasColumnName("liked_at")
            .IsRequired();

        builder.HasOne(rl => rl.Review)
            .WithMany(r => r.ReviewLikes)
            .HasForeignKey(rl => rl.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rl => rl.User)
            .WithMany(u => u.ReviewLikes)
            .HasForeignKey(rl => rl.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}