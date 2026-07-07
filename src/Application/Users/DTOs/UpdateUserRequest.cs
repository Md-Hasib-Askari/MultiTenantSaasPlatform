using Application.Common;

namespace Application.Users.DTOs;

[AtLeastOneRequired(nameof(DisplayName), nameof(Email))]
public record UpdateUserRequest(string? DisplayName, string? Email);
