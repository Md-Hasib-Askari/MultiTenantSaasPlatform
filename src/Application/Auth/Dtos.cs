using System.ComponentModel.DataAnnotations;

namespace Application.Auth;

public record RegisterRequest(
    [Required] [EmailAddress] string Email,
    [Required] string Password,
    [Required] string DisplayName,
    [Required] string TenantSlug
) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Password.Length < 8)
            yield return new("Password must be at least 8 characters.", [nameof(Password)]);
        if (!Password.Any(char.IsUpper))
            yield return new("Password must contain an uppercase letter.", [nameof(Password)]);
        if (!Password.Any(char.IsLower))
            yield return new("Password must contain a lowercase letter.", [nameof(Password)]);
        if (!Password.Any(char.IsDigit))
            yield return new("Password must contain a digit.", [nameof(Password)]);
        if (!Password.Any(c => !char.IsLetterOrDigit(c)))
            yield return new("Password must contain a non-alphanumeric character.", [nameof(Password)]);
    }
}

public record LoginRequest([Required] [EmailAddress] string Email, [Required] string Password);

public record TokenResponse(string AccessToken, string RefreshToken);

public record RefreshRequest([Required] string RefreshToken);
