using System.Net.Mail;

namespace obragris_api.domain.shared;

public static class InputValidator
{
    public static string Sanitize(string? input, string fieldName, bool isOptional = false)
    {
        if (input == null)
        {
            if (isOptional)
                return string.Empty;
            throw new ArgumentNullException(nameof(input), $"{fieldName} cannot be null");
        }

        // Check for dangerous characters that could be used in injection attacks
        if (input.IndexOfAny(ValidationConstants.DangerousChars) >= 0)
            throw new ArgumentException($"{fieldName} contains invalid characters", fieldName);

        // Check for null bytes (could truncate strings in database)
        if (input.Contains('\0'))
            throw new ArgumentException($"{fieldName} contains null bytes", fieldName);

        // Trim but check if result would be empty
        var trimmed = input.Trim();
        
        // Skip length check for optional empty fields
        if (isOptional && string.IsNullOrEmpty(trimmed))
            return string.Empty;

        // Prevent excessively long inputs that could cause DoS
        if (trimmed.Length > ValidationConstants.MaxInputLength)
            throw new ArgumentException($"{fieldName} exceeds maximum length of {ValidationConstants.MaxInputLength}", fieldName);

        if (trimmed.Length < ValidationConstants.MinInputLength)
            throw new ArgumentException($"{fieldName} must be at least {ValidationConstants.MinInputLength} character", fieldName);

        return trimmed;
    }

    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Check for multiple @ symbols
            var atCount = email.Count(c => c == '@');
            if (atCount != 1)
                return false;

            // Check for dangerous characters that could be used in injection
            if (email.Contains('\0') || email.Contains('\r') || email.Contains('\n'))
                return false;

            var addr = new MailAddress(email);
            
            // Verify the address normalizes correctly
            if (string.IsNullOrEmpty(addr.Address) || string.IsNullOrEmpty(addr.User))
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }
}
