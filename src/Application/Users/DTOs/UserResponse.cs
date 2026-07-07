using Domain.Enums;

namespace Application.Users.DTOs;

public record UserResponse(
    Guid Id,
    string Email,
    string UserName,
    string DisplayName,
    UserStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);
