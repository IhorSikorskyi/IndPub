using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("subscriptions", t => t.HasCheckConstraint(
            "ck_subscriptions_user_not_author",
            "user_id <> author_id"));

        builder.HasKey(s => new { s.UserId, s.AuthorId })
            .HasName("pk_subscriptions");

        builder.Property(s => s.UserId)
            .HasColumnName("user_id");

        builder.Property(s => s.AuthorId)
            .HasColumnName("author_id");

        builder.Property(s => s.SubscribedAt)
            .HasColumnName("subscribed_at")
            .IsRequired();

        builder.HasOne(s => s.User)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Author)
            .WithMany(a => a.Subscribers)
            .HasForeignKey(s => s.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}