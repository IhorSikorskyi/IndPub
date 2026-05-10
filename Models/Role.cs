using System.ComponentModel.DataAnnotations;

namespace IndPubBack.Models;

public class Role : BaseEntity
{
    public Role(Guid id) : base(id) { }
    public Role() : base() { }

    [MinLength(1)]
    public required string Name { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}