namespace obragris_api.domain.shared;

public static class ValidationConstants
{
    public const int MaxInputLength = 500;
    public const int MinInputLength = 1;
    
    public static readonly char[] DangerousChars = { '\0', '\r', '\n', '\t', '\v', '\f' };
    
    public const int EmailMaxLength = 254;
    public const int EmailMinLength = 3;
    public const int LocalPartMaxLength = 64;
}
