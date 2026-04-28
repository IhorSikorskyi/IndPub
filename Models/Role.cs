namespace IndPubBack.Models;

public class Role : BaseEntity
{
    public Role(Guid id) : base(id) { }
    public Role() : base() { }

    public string Name { get; set; } = null!;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}