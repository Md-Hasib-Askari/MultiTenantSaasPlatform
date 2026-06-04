using Domain.Entities.Common;

namespace Domain.Entities;

public class Project : BaseAudit
{
    public Guid Id { get; private init; }
    public Guid TenantId { get; private init; }
    public string Name { get; private set; } = null!;

    private Project() { }

    public static Project Create(Guid tenantId, string name) =>
        new Project
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name,
            CreatedAt = DateTimeOffset.UtcNow,
        };
}
