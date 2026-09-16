using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Entities;

public class RefreshToken : BaseEntity
{

    [MinLength(1)]
    public required string RefreshTokenHash { get; set; }
    public DateTime? RevokedAt { get; set; }
    public required DateTime RefreshTokenExpiry { get; set; }
    public Guid? ReplacedByTokenId { get; set; }
    public RefreshToken? ReplacedByToken { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}