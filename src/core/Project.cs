namespace obragris_api.core;

public class Project : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;

    //---Constructor Entity---//
    private Project()
    {
    }

    //---Creador de proyectos---//
    public Project(string name, string description, string location)
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
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentNullException(nameof(description));
    }
}