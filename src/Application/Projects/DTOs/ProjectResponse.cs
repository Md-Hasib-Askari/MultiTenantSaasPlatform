namespace Application.Projects.DTOs;

public record ProjectResponse(
    Guid Id,
    Guid TenantId,
    string Name,
    string? Description,
    string? Color,
    bool IsActive,
    DateTimeOffset CreatedAt,
    Guid CreatedById,
    DateTimeOffset? UpdatedAt,
    Guid? UpdatedById
);
