namespace obragris_api.domain.entities;

public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; } = default!;
    public Guid TenantId { get; set; }
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public string? CreatedBy { get; protected set; }
    public DateTime? LastModifiedAt { get; protected set; }
    public string? LastModifiedBy { get; protected set; }
    
    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    public void UndoDelete()
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}