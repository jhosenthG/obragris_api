using obragris_api.domain.enums;
using obragris_api.domain.shared;

namespace obragris_api.domain.entities;

public class Project : BaseEntity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? ClientName { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public ProjectStatus Status { get; private set; } = ProjectStatus.Pending;
    public decimal? Budget { get; private set; }

    // Foreign Keys
    public Guid CompanyId { get; private set; }

    // Navigation properties
    private readonly List<Report> _reports = new();
    public IReadOnlyCollection<Report> Reports => _reports.AsReadOnly();

    private Project()
    {
        // Required by EF Core
    }

    public Project(
        string name,
        Guid companyId,
        Guid tenantId,
        string? description = null,
        string? clientName = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        decimal? budget = null)
    {
        var sanitizedName = InputValidator.Sanitize(name, nameof(name));
        var sanitizedDescription = InputValidator.Sanitize(description, nameof(description), isOptional: true);
        var sanitizedClientName = InputValidator.Sanitize(clientName, nameof(clientName), isOptional: true);

        ValidateRequiredFields(sanitizedName, companyId);

        Id = Guid.NewGuid();
        TenantId = tenantId;
        CompanyId = companyId;
        Name = sanitizedName.Trim();
        Description = string.IsNullOrEmpty(sanitizedDescription) ? null : sanitizedDescription.Trim();
        ClientName = string.IsNullOrEmpty(sanitizedClientName) ? null : sanitizedClientName.Trim();
        StartDate = startDate;
        EndDate = endDate;
        Budget = budget;
        Status = ProjectStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string name,
        string? description,
        string? clientName,
        DateTime? startDate,
        DateTime? endDate,
        decimal? budget)
    {
        var sanitizedName = InputValidator.Sanitize(name, nameof(name));
        var sanitizedDescription = InputValidator.Sanitize(description, nameof(description), isOptional: true);
        var sanitizedClientName = InputValidator.Sanitize(clientName, nameof(clientName), isOptional: true);

        ValidateRequiredFields(sanitizedName, CompanyId);

        Name = sanitizedName.Trim();
        Description = string.IsNullOrEmpty(sanitizedDescription) ? null : sanitizedDescription.Trim();
        ClientName = string.IsNullOrEmpty(sanitizedClientName) ? null : sanitizedClientName.Trim();
        StartDate = startDate;
        EndDate = endDate;
        Budget = budget;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ProjectStatus status)
    {
        Status = status;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Start()
    {
        if (Status != ProjectStatus.Pending)
            throw new InvalidOperationException("Project can only be started from Pending status");

        Status = ProjectStatus.InProgress;
        StartDate ??= DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != ProjectStatus.InProgress)
            throw new InvalidOperationException("Project can only be completed from In Progress status");

        Status = ProjectStatus.Completed;
        EndDate ??= DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == ProjectStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed project");

        Status = ProjectStatus.Cancelled;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Pause()
    {
        if (Status != ProjectStatus.InProgress)
            throw new InvalidOperationException("Project can only be paused from In Progress status");

        Status = ProjectStatus.OnHold;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Resume()
    {
        if (Status != ProjectStatus.OnHold)
            throw new InvalidOperationException("Project can only be resumed from On Hold status");

        Status = ProjectStatus.InProgress;
        LastModifiedAt = DateTime.UtcNow;
    }

    private static void ValidateRequiredFields(string name, Guid companyId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name is required", nameof(name));

        if (companyId == Guid.Empty)
            throw new ArgumentException("Company ID is required", nameof(companyId));
    }
}
