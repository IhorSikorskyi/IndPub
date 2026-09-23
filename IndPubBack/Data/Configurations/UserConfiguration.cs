using IndPubBack.Entities;
using IndPubBack.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        base.Configure(builder);

        builder.Property(u => u.Id)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(u => u.Login)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("login");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(254)
            .HasColumnName("email");

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(256)
            .HasColumnName("password_hash");

        builder.Property(u => u.CreatedAt)
            .HasColumnName("joined_at")
            .IsRequired();

        builder.Property(u => u.Bio)
            .HasMaxLength(1000)
            .HasColumnName("bio");

        builder.Property(u => u.ProfilePictureUrl)
            .HasMaxLength(2048)
            .HasColumnName("profile_picture_url");

        builder.Property(u => u.Role)
            .HasColumnName("role")
            .HasConversion<string>()
            .HasDefaultValue(Roles.User)
            .IsRequired();
    }
}