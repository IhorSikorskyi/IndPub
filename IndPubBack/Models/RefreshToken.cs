using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Models;

public class RefreshToken : BaseEntity
{
    public RefreshToken() : base()
    {

    }
    public Guid UserId { get; set; }

    [MaxLength(256)]
    public required string RefreshTokenHash { get; set; }
    public DateTime? RevokedAt { get; set; }
    public required DateTime RefreshTokenExpiry { get; set; }
    public Guid? ReplacedByTokenId { get; set; }

    public User User { get; set; } = null!;
}