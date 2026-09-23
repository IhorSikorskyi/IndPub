using IndPubBack.Entities;
using IndPubBack.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class NotificationConfiguration : BaseEntityConfiguration<Notification>
{
    public override void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications", n => n.HasCheckConstraint(
            "ck_notifications_chapter_xor_author",
            "(chapter_id IS NOT NULL AND author_id IS NULL) OR (chapter_id IS NULL AND author_id IS NOT NULL AND user_id <> author_id)"));

        base.Configure(builder);

        builder.Property(n => n.Message)
            .HasColumnName("message")
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(n => n.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasDefaultValue(NotificationType.NewChapter)
            .IsRequired();

        builder.Property(n => n.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(n => n.BookId)
            .HasColumnName("book_id")
            .IsRequired();

        builder.Property(n => n.AuthorId)
            .HasColumnName("author_id");

        builder.Property(n => n.ChapterId)
            .HasColumnName("chapter_id");

        // Recipient: deleting a user removes notifications sent to them.
        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Author: two FKs to User (UserId, AuthorId) — avoid double-cascade
        // ambiguity on the User table by restricting this side.
        builder.HasOne(n => n.Author)
            .WithMany(u => u.AuthoredNotifications)
            .HasForeignKey(n => n.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Book: keep as the single cascade path into Notification.
        builder.HasOne(n => n.Book)
            .WithMany(b => b.Notifications)
            .HasForeignKey(n => n.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        // Chapter: restrict so Book -> Chapter -> Notification isn't a
        // second cascade path alongside Book -> Notification directly.
        builder.HasOne(n => n.Chapter)
            .WithMany(c => c.Notifications)
            .HasForeignKey(n => n.ChapterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}