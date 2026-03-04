using obragris_api.domain.enums;
using obragris_api.domain.shared;

namespace obragris_api.domain.entities;

public class Report : BaseEntity<Guid>
{
    public string Title { get; private set; } = string.Empty;
    public string? Summary { get; private set; }
    public string? Content { get; private set; }
    public ReportType Type { get; private set; }
    public ReportStatus Status { get; private set; } = ReportStatus.Draft;
    public DateTime ReportDate { get; private set; }
    public DateTime? SubmittedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    public Guid ProjectId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public Guid? ApprovedByUserId { get; private set; }

    private readonly List<ReportTask> _tasks = new();
    public IReadOnlyCollection<ReportTask> Tasks => _tasks.AsReadOnly();

    private Report()
    {
    }

    public Report(
        string title,
        Guid projectId,
        Guid createdByUserId,
        Guid tenantId,
        ReportType type,
        DateTime reportDate,
        string? summary = null,
        string? content = null)
    {
        var sanitizedTitle = InputValidator.Sanitize(title, nameof(title));
        var sanitizedSummary = InputValidator.Sanitize(summary, nameof(summary), isOptional: true);
        var sanitizedContent = InputValidator.Sanitize(content, nameof(content), isOptional: true);

        ValidateRequiredFields(sanitizedTitle, projectId, createdByUserId, type);
        ValidateReportDate(reportDate);

        Id = Guid.NewGuid();
        TenantId = tenantId;
        ProjectId = projectId;
        CreatedByUserId = createdByUserId;
        Title = sanitizedTitle.Trim();
        Summary = string.IsNullOrEmpty(sanitizedSummary) ? null : sanitizedSummary.Trim();
        Content = string.IsNullOrEmpty(sanitizedContent) ? null : sanitizedContent.Trim();
        Type = type;
        ReportDate = reportDate;
        Status = ReportStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateContent(string title, string? summary, string? content)
    {
        if (Status != ReportStatus.Draft)
            throw new InvalidOperationException("Can only update content in Draft status");

        var sanitizedTitle = InputValidator.Sanitize(title, nameof(title));
        var sanitizedSummary = InputValidator.Sanitize(summary, nameof(summary), isOptional: true);
        var sanitizedContent = InputValidator.Sanitize(content, nameof(content), isOptional: true);

        ValidateRequiredFields(sanitizedTitle, ProjectId, CreatedByUserId, Type);

        Title = sanitizedTitle.Trim();
        Summary = string.IsNullOrEmpty(sanitizedSummary) ? null : sanitizedSummary.Trim();
        Content = string.IsNullOrEmpty(sanitizedContent) ? null : sanitizedContent.Trim();
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Submit()
    {
        if (Status != ReportStatus.Draft)
            throw new InvalidOperationException("Can only submit Draft reports");

        Status = ReportStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Approve(Guid approvedByUserId)
    {
        if (Status != ReportStatus.Submitted)
            throw new InvalidOperationException("Can only approve Submitted reports");

        if (approvedByUserId == Guid.Empty)
            throw new ArgumentException("Approver user ID is required", nameof(approvedByUserId));

        Status = ReportStatus.Approved;
        ApprovedByUserId = approvedByUserId;
        ApprovedAt = DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Reject(Guid approvedByUserId, string reason)
    {
        if (Status != ReportStatus.Submitted)
            throw new InvalidOperationException("Can only reject Submitted reports");

        if (approvedByUserId == Guid.Empty)
            throw new ArgumentException("Rejector user ID is required", nameof(approvedByUserId));

        var sanitizedReason = InputValidator.Sanitize(reason, nameof(reason));
        if (string.IsNullOrWhiteSpace(sanitizedReason))
            throw new ArgumentException("Rejection reason is required", nameof(reason));

        Status = ReportStatus.Rejected;
        ApprovedByUserId = approvedByUserId;
        RejectedAt = DateTime.UtcNow;
        RejectionReason = sanitizedReason.Trim();
        LastModifiedAt = DateTime.UtcNow;
    }

    public void ReturnToDraft()
    {
        if (Status == ReportStatus.Approved)
            throw new InvalidOperationException("Cannot return approved report to draft");

        Status = ReportStatus.Draft;
        SubmittedAt = null;
        ApprovedAt = null;
        RejectedAt = null;
        RejectionReason = null;
        ApprovedByUserId = null;
        LastModifiedAt = DateTime.UtcNow;
    }

    private static void ValidateRequiredFields(string title, Guid projectId, Guid createdByUserId, ReportType type)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Report title is required", nameof(title));

        if (projectId == Guid.Empty)
            throw new ArgumentException("Project ID is required", nameof(projectId));

        if (createdByUserId == Guid.Empty)
            throw new ArgumentException("Creator user ID is required", nameof(createdByUserId));

        if (!Enum.IsDefined(typeof(ReportType), type))
            throw new ArgumentException("Invalid report type", nameof(type));
    }

    private static void ValidateReportDate(DateTime reportDate)
    {
        if (reportDate > DateTime.UtcNow.AddYears(1))
            throw new ArgumentException("Report date cannot be more than one year in the future", nameof(reportDate));
    }
}
