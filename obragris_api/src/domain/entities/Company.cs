using obragris_api.domain.shared;

namespace obragris_api.domain.entities;

public class Company : BaseEntity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? TaxId { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public string? Website { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    private readonly List<Project> _projects = new();
    public IReadOnlyCollection<Project> Projects => _projects.AsReadOnly();

    private readonly List<User> _users = new();
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    private Company()
    {
        // Required by EF Core
    }

    public Company(
        string name,
        Guid tenantId,
        string? description = null,
        string? taxId = null,
        string? phoneNumber = null,
        string? email = null,
        string? address = null,
        string? website = null)
    {
        var sanitizedName = InputValidator.Sanitize(name, nameof(name));
        var sanitizedDescription = InputValidator.Sanitize(description, nameof(description), isOptional: true);
        var sanitizedTaxId = InputValidator.Sanitize(taxId, nameof(taxId), isOptional: true);
        var sanitizedPhoneNumber = InputValidator.Sanitize(phoneNumber, nameof(phoneNumber), isOptional: true);
        var sanitizedEmail = InputValidator.Sanitize(email, nameof(email), isOptional: true);
        if (!string.IsNullOrEmpty(sanitizedEmail) && !InputValidator.IsValidEmail(sanitizedEmail))
            throw new ArgumentException("Invalid email format", nameof(email));
        var sanitizedAddress = InputValidator.Sanitize(address, nameof(address), isOptional: true);
        var sanitizedWebsite = InputValidator.Sanitize(website, nameof(website), isOptional: true);

        ValidateRequiredFields(sanitizedName);

        Id = Guid.NewGuid();
        TenantId = tenantId;
        Name = sanitizedName.Trim();
        Description = string.IsNullOrEmpty(sanitizedDescription) ? null : sanitizedDescription.Trim();
        TaxId = string.IsNullOrEmpty(sanitizedTaxId) ? null : sanitizedTaxId.Trim();
        PhoneNumber = string.IsNullOrEmpty(sanitizedPhoneNumber) ? null : sanitizedPhoneNumber.Trim();
        Email = string.IsNullOrEmpty(sanitizedEmail) ? null : sanitizedEmail.Trim();
        Address = string.IsNullOrEmpty(sanitizedAddress) ? null : sanitizedAddress.Trim();
        Website = string.IsNullOrEmpty(sanitizedWebsite) ? null : sanitizedWebsite.Trim();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string name,
        string? description,
        string? taxId,
        string? phoneNumber,
        string? email,
        string? address,
        string? website)
    {
        var sanitizedName = InputValidator.Sanitize(name, nameof(name));
        var sanitizedDescription = InputValidator.Sanitize(description, nameof(description), isOptional: true);
        var sanitizedTaxId = InputValidator.Sanitize(taxId, nameof(taxId), isOptional: true);
        var sanitizedPhoneNumber = InputValidator.Sanitize(phoneNumber, nameof(phoneNumber), isOptional: true);
        var sanitizedEmail = InputValidator.Sanitize(email, nameof(email), isOptional: true);
        if (!string.IsNullOrEmpty(sanitizedEmail) && !InputValidator.IsValidEmail(sanitizedEmail))
            throw new ArgumentException("Invalid email format", nameof(email));
        var sanitizedAddress = InputValidator.Sanitize(address, nameof(address), isOptional: true);
        var sanitizedWebsite = InputValidator.Sanitize(website, nameof(website), isOptional: true);

        ValidateRequiredFields(sanitizedName);

        Name = sanitizedName.Trim();
        Description = string.IsNullOrEmpty(sanitizedDescription) ? null : sanitizedDescription.Trim();
        TaxId = string.IsNullOrEmpty(sanitizedTaxId) ? null : sanitizedTaxId.Trim();
        PhoneNumber = string.IsNullOrEmpty(sanitizedPhoneNumber) ? null : sanitizedPhoneNumber.Trim();
        Email = string.IsNullOrEmpty(sanitizedEmail) ? null : sanitizedEmail.Trim();
        Address = string.IsNullOrEmpty(sanitizedAddress) ? null : sanitizedAddress.Trim();
        Website = string.IsNullOrEmpty(sanitizedWebsite) ? null : sanitizedWebsite.Trim();
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

    private static void ValidateRequiredFields(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Company name is required", nameof(name));
    }
}
