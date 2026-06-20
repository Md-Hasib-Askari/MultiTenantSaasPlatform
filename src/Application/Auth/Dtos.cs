using System.ComponentModel.DataAnnotations;

namespace Application.Auth;

public record RegisterRequest(
    [Required] [EmailAddress] string Email,
    [Required] [MinLength(8)] string Password,
    [Required] string DisplayName,
    [Required] string TenantSlug
);

public record LoginRequest([Required] [EmailAddress] string Email, [Required] string Password);

public record TokenResponse(string AccessToken, string RefreshToken);

public record RefreshRequest([Required] string RefreshToken);
