using Domain.Entities.Common;
using Domain.Entities.Tasks;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Domain.Entities.Projects;

public class Project : BaseAudit, ITenantScoped
{
    private const int MaxNameLength = 100;

    public Guid Id { get; private init; }
    public Guid TenantId { get; private init; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? Color { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public Tenant Tenant { get; private set; } = null!;
    public ICollection<TaskItem> Tasks { get; private set; } = [];
    public ICollection<ProjectMember> Members { get; private set; } = [];

    private Project() { }

    public static Project Create(Guid tenantId, string name, string? description, string? color, Guid createdById)
    {
        ValidateName(name);

        return new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name,
            Description = description,
            Color = ValidateColor(color) ?? "#63B3ED",
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedById = createdById,
        };
    }

    public void UpdateDetails(string? name, string? description, string? color)
    {
        if (name is not null)
        {
            ValidateName(name);
            Name = name;
        }

        if (description is not null)
            Description = description;

        if (color is not null)
            Color = ValidateColor(color) ?? Color;

        if (name is not null || description is not null || color is not null)
            UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ChangeStatus(bool isActive)
    {
        if (IsActive == isActive)
            return;

        IsActive = isActive;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException(new() { ["Name"] = ["Name is required."] });

        if (name.Length > MaxNameLength)
            throw new ValidationException(
                new() { ["Name"] = [$"Name must not exceed {MaxNameLength} characters."] }
            );
    }

    private static string? ValidateColor(string? color)
    {
        if (color is null)
            return null;

        if (
            color.Length is not (4 or 7)
            || !color.StartsWith('#')
            || !color[1..].All(c => char.IsAsciiHexDigit(c))
        )
            throw new ValidationException(
                new() { ["Color"] = ["Color must be a valid hex (e.g. #63B3ED or #FFF)."] }
            );

        return color;
    }
}
