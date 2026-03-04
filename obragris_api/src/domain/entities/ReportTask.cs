using obragris_api.domain.enums;
using obragris_api.domain.shared;
using TaskStatus = obragris_api.domain.enums.TaskStatus;

namespace obragris_api.domain.entities;

public class ReportTask : BaseEntity<Guid>
{
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskPriority Priority { get; private set; } = TaskPriority.Medium;
    public TaskStatus Status { get; private set; } = TaskStatus.Pending;
    public TaskCategory Category { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public int EstimatedHours { get; private set; }
    public int ActualHours { get; private set; }
    public decimal? Cost { get; private set; }

    public Guid ReportId { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public Guid? ParentTaskId { get; private set; }

    private readonly List<ReportTask> _subTasks = new();
    public IReadOnlyCollection<ReportTask> SubTasks => _subTasks.AsReadOnly();

    private ReportTask()
    {
    }

    public ReportTask(
        string title,
        Guid reportId,
        Guid tenantId,
        TaskCategory category,
        string? description = null,
        TaskPriority priority = TaskPriority.Medium,
        DateTime? dueDate = null,
        int estimatedHours = 0,
        Guid? assignedToUserId = null,
        Guid? parentTaskId = null)
    {
        var sanitizedTitle = InputValidator.Sanitize(title, nameof(title));
        var sanitizedDescription = InputValidator.Sanitize(description, nameof(description), isOptional: true);

        ValidateRequiredFields(sanitizedTitle, reportId, category);
        ValidateEstimatedHours(estimatedHours);

        Id = Guid.NewGuid();
        TenantId = tenantId;
        ReportId = reportId;
        Title = sanitizedTitle.Trim();
        Description = string.IsNullOrEmpty(sanitizedDescription) ? null : sanitizedDescription.Trim();
        Category = category;
        Priority = priority;
        DueDate = dueDate;
        EstimatedHours = estimatedHours;
        AssignedToUserId = assignedToUserId;
        ParentTaskId = parentTaskId;
        Status = TaskStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string title,
        string? description,
        TaskPriority priority,
        TaskCategory category,
        DateTime? dueDate,
        int estimatedHours)
    {
        var sanitizedTitle = InputValidator.Sanitize(title, nameof(title));
        var sanitizedDescription = InputValidator.Sanitize(description, nameof(description), isOptional: true);

        ValidateRequiredFields(sanitizedTitle, ReportId, category);
        ValidateEstimatedHours(estimatedHours);

        Title = sanitizedTitle.Trim();
        Description = string.IsNullOrEmpty(sanitizedDescription) ? null : sanitizedDescription.Trim();
        Priority = priority;
        Category = category;
        DueDate = dueDate;
        EstimatedHours = estimatedHours;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void AssignTo(Guid? userId)
    {
        AssignedToUserId = userId;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Start()
    {
        if (Status == TaskStatus.Completed)
            throw new InvalidOperationException("Cannot start a completed task");

        if (Status == TaskStatus.InProgress)
            throw new InvalidOperationException("Task is already in progress");

        Status = TaskStatus.InProgress;
        StartedAt ??= DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status == TaskStatus.Pending)
            throw new InvalidOperationException("Cannot complete a pending task");

        if (_subTasks.Count > 0 && _subTasks.Any(s => s.Status != TaskStatus.Completed))
            throw new InvalidOperationException("Cannot complete task with incomplete sub-tasks");

        Status = TaskStatus.Completed;
        CompletedAt ??= DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == TaskStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed task");

        Status = TaskStatus.Cancelled;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Reopen()
    {
        if (Status != TaskStatus.Completed && Status != TaskStatus.Cancelled)
            throw new InvalidOperationException("Can only reopen completed or cancelled tasks");

        Status = TaskStatus.Pending;
        CompletedAt = null;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void UpdateProgress(int actualHours, decimal? cost = null)
    {
        if (actualHours < 0)
            throw new ArgumentException("Actual hours cannot be negative", nameof(actualHours));

        if (cost.HasValue && cost.Value < 0)
            throw new ArgumentException("Cost cannot be negative", nameof(cost));

        ActualHours = actualHours;
        Cost = cost;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void ChangePriority(TaskPriority priority)
    {
        if (!Enum.IsDefined(typeof(TaskPriority), priority))
            throw new ArgumentException("Invalid priority", nameof(priority));

        Priority = priority;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void AddSubTask(ReportTask subTask)
    {
        if (subTask == null)
            throw new ArgumentNullException(nameof(subTask));

        if (subTask.Id == Id)
            throw new InvalidOperationException("A task cannot be its own parent");

        if (Status == TaskStatus.Completed)
            throw new InvalidOperationException("Cannot add sub-tasks to a completed task");

        _subTasks.Add(subTask);
        LastModifiedAt = DateTime.UtcNow;
    }

    private static void ValidateRequiredFields(string title, Guid reportId, TaskCategory category)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Task title is required", nameof(title));

        if (reportId == Guid.Empty)
            throw new ArgumentException("Report ID is required", nameof(reportId));

        if (!Enum.IsDefined(typeof(TaskCategory), category))
            throw new ArgumentException("Invalid task category", nameof(category));
    }

    private static void ValidateEstimatedHours(int estimatedHours)
    {
        if (estimatedHours < 0)
            throw new ArgumentException("Estimated hours cannot be negative", nameof(estimatedHours));
    }
}
