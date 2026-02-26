using obragris_api.domain.enums;
using obragris_api.domain.shared;

namespace obragris_api.domain.entities;

public class User : BaseEntity<Guid>
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? JobTitle { get; private set; }
    public string? PhoneNumber { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? LastLoginAt { get; private set; }
    public SubscriptionStatus SubscriptionStatus { get; private set; } = SubscriptionStatus.Active;
    public DateTime? SubscriptionExpiresAt { get; private set; }

    // Foreign Keys
    public Guid CompanyId { get; private set; }

    // Navigation properties
    private readonly List<Project> _assignedProjects = new();
    public IReadOnlyCollection<Project> AssignedProjects => _assignedProjects.AsReadOnly();

    private readonly List<Report> _reports = new();
    public IReadOnlyCollection<Report> Reports => _reports.AsReadOnly();

    private User()
    {
        // Required by EF Core
    }

    public User(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        Guid tenantId,
        Guid companyId,
        string? jobTitle = null,
        string? phoneNumber = null)
    {
        var sanitizedFirstName = InputValidator.Sanitize(firstName, nameof(firstName));
        var sanitizedLastName = InputValidator.Sanitize(lastName, nameof(lastName));
        var sanitizedEmail = InputValidator.Sanitize(email, nameof(email));
        var sanitizedPasswordHash = InputValidator.Sanitize(passwordHash, nameof(passwordHash));
        var sanitizedJobTitle = InputValidator.Sanitize(jobTitle, nameof(jobTitle), isOptional: true);
        var sanitizedPhoneNumber = InputValidator.Sanitize(phoneNumber, nameof(phoneNumber), isOptional: true);

        ValidateRequiredFields(sanitizedFirstName, sanitizedLastName, sanitizedEmail, sanitizedPasswordHash);

        if (companyId == Guid.Empty)
            throw new ArgumentException("Company ID is required", nameof(companyId));

        Id = Guid.NewGuid();
        TenantId = tenantId;
        CompanyId = companyId;
        FirstName = sanitizedFirstName.Trim();
        LastName = sanitizedLastName.Trim();
        Email = sanitizedEmail.Trim().ToLowerInvariant();
        PasswordHash = sanitizedPasswordHash;
        JobTitle = string.IsNullOrEmpty(sanitizedJobTitle) ? null : sanitizedJobTitle.Trim();
        PhoneNumber = string.IsNullOrEmpty(sanitizedPhoneNumber) ? null : sanitizedPhoneNumber.Trim();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName, string? jobTitle, string? phoneNumber)
    {
        var sanitizedFirstName = InputValidator.Sanitize(firstName, nameof(firstName));
        var sanitizedLastName = InputValidator.Sanitize(lastName, nameof(lastName));
        var sanitizedJobTitle = InputValidator.Sanitize(jobTitle, nameof(jobTitle), isOptional: true);
        var sanitizedPhoneNumber = InputValidator.Sanitize(phoneNumber, nameof(phoneNumber), isOptional: true);

        ValidateRequiredFields(sanitizedFirstName, sanitizedLastName, Email, PasswordHash);

        FirstName = sanitizedFirstName.Trim();
        LastName = sanitizedLastName.Trim();
        JobTitle = string.IsNullOrEmpty(sanitizedJobTitle) ? null : sanitizedJobTitle.Trim();
        PhoneNumber = string.IsNullOrEmpty(sanitizedPhoneNumber) ? null : sanitizedPhoneNumber.Trim();
        LastModifiedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(string newEmail)
    {
        var sanitizedEmail = InputValidator.Sanitize(newEmail, nameof(newEmail));

        if (!InputValidator.IsValidEmail(sanitizedEmail))
            throw new ArgumentException("Invalid email format", nameof(newEmail));

        Email = sanitizedEmail.Trim().ToLowerInvariant();
        LastModifiedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        var sanitizedPassword = InputValidator.Sanitize(newPasswordHash, nameof(newPasswordHash));

        if (string.IsNullOrWhiteSpace(sanitizedPassword))
            throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));

        PasswordHash = sanitizedPassword;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public string GetFullName() => $"{FirstName} {LastName}";

    private static void ValidateRequiredFields(string firstName, string lastName, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required", nameof(lastName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (!InputValidator.IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required", nameof(passwordHash));
    }

    public void ActivateSubscription(DateTime? expiresAt = null)
    {
        SubscriptionStatus = SubscriptionStatus.Active;
        SubscriptionExpiresAt = expiresAt;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void CancelSubscription()
    {
        SubscriptionStatus = SubscriptionStatus.Cancelled;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void SuspendSubscription()
    {
        SubscriptionStatus = SubscriptionStatus.Suspended;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void ExtendSubscription(DateTime newExpirationDate)
    {
        if (newExpirationDate <= DateTime.UtcNow)
            throw new ArgumentException("Expiration date must be in the future", nameof(newExpirationDate));

        SubscriptionExpiresAt = newExpirationDate;
        if (SubscriptionStatus == SubscriptionStatus.Expired)
            SubscriptionStatus = SubscriptionStatus.Active;
        
        LastModifiedAt = DateTime.UtcNow;
    }

    public bool IsSubscriptionValid()
    {
        return SubscriptionStatus == SubscriptionStatus.Active 
            && (!SubscriptionExpiresAt.HasValue || SubscriptionExpiresAt.Value > DateTime.UtcNow);
    }
}
