namespace obragris_api.core;

public class Project : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    
    public Guid UserId { get; private set; }
    public virtual User User { get; private set; } = null!;
    
    public virtual ICollection<Report> Reports { get; private set; } = new HashSet<Report>();

    //---Constructor Entity---//
    private Project()
    {
    }

    //---Creador de proyectos---//
    public Project(string name, string description, string location, Guid userId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Location = location;
        
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }
    
    //---Metodos de negocios---//
    public void Update(string name, string description, string location)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del proyecto no puede estar vacío.");
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("La descripción del proyecto no puede estar vacía.");
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("La ubicación del proyecto no puede estar vacía.");

        Name = name;
        Description = description;
        Location = location;
    }
}