namespace obragris_api.core;

public class Task : BaseEntity
{
    public string Description { get; private set; } = string.Empty;
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    
    //---Relaciones---//
    public Guid ProjectId { get; private set; }
    public virtual Project Project { get; private set; } = null!;
    
    private Task() {}
    
    public Task(string description, Guid projectId)
    {
        Id = Guid.NewGuid();
        Description = description;
        IsCompleted = false;
        CompletedAt = null;
        
        ProjectId = projectId;

        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }
    
    public void MarkAsCompleted()
    {
        if (IsCompleted)
            throw new InvalidOperationException("La tarea ya está marcada como completada.");
        
        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
    }
    public void UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("La descripción de la tarea no puede estar vacía.");
        
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }
}