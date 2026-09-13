namespace IndPubBack.Entities;

public class BaseEntity
{
    protected BaseEntity(Guid id)
    {
        this.Id = id;
    }

    protected BaseEntity()
    {
        this.Id = Guid.Empty;
    }

    public Guid Id { get; set; }
}