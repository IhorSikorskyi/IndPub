using IndPubBack.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IndPubBack.Data.Configurations;

public class RefreshTokenConfiguration : BaseEntityConfiguration<RefreshToken>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        base.Configure(builder);

        builder.Property(rt => rt.Id)
            .HasColumnName("refresh_token_id")
            .IsRequired();

        builder.Property(rt => rt.RefreshTokenHash)
            .IsRequired()
            .HasMaxLength(256)
            .HasColumnName("refresh_token_hash");

        builder.Property(rt => rt.RevokedAt)
            .HasColumnName("revoked_at");

        builder.Property(rt => rt.RefreshTokenExpiry)
            .IsRequired()
            .HasColumnName("refresh_token_expiry");
        
        builder.Property(rt => rt.ReplacedByTokenId)
            .HasColumnName("replaced_by_refresh_token_id");

        builder.HasOne(rt => rt.ReplacedByToken)
            .WithMany()
            .HasForeignKey(rt => rt.ReplacedByTokenId)
            .OnDelete(DeleteBehavior.Restrict) // avoid self-referencing cascade cycles
            .IsRequired(false);

        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .IsRequired();
    }
}