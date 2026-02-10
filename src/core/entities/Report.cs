namespace obragris_api.core;

public class Report : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string ImageUrl { get; private set; } = string.Empty;
    public string Observation { get; private set; } = string.Empty;

    public DateTime ReportDate { get; private set; }

    //---Relaciones---//
    public Guid ProjectId { get; private set; }
    public virtual Project Project { get; private set; } = null!;

    //---Constructor Entity---//
    private Report()
    {
    }

    //---Creador de reportes---//
    public Report(string title, string description, string imageUrl, string observation, DateTime reportDate,
        Guid projectId)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        ImageUrl = imageUrl;
        Observation = observation;
        ReportDate = reportDate;

        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    //---Metodos de negocios---//
    public void Update(string title, string description, string imageUrl, string observation, DateTime reportDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("El título del reporte no puede estar vacío.");
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("La descripción del reporte no puede estar vacía.");
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("La URL de la imagen del reporte no puede estar vacía.");
        if (string.IsNullOrWhiteSpace(observation))
            throw new ArgumentException("La observación del reporte no puede estar vacía.");
        if (reportDate > DateTime.UtcNow)
            throw new ArgumentException("La fecha del reporte no puede ser futura.");
    }
}