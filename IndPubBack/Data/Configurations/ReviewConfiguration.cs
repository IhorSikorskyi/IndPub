using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class ReviewConfiguration : BaseEntityConfiguration<Review>
{
    public override void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");

        base.Configure(builder);

        builder.Property(r => r.Rating)
            .IsRequired()
            .HasPrecision(3, 2)
            .HasColumnName("rating");

        builder.Property(r => r.Text)
            .IsRequired()
            .HasMaxLength(10000)
            .HasColumnName("text");

        builder.Property(r => r.BookId)
            .IsRequired()
            .HasColumnName("book_id");

        builder.Property(r => r.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.HasOne(r => r.Book)
            .WithMany(b => b.Reviews)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship with Comments has been defined in CommentConfiguration,
        // so we don't need to define it here.
    }
}