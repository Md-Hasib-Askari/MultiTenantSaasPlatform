namespace Domain.Entities.Common;

/// <summary>
/// Contract for entities that track creation, update, and soft-delete timestamps.
/// Implemented by both <see cref="BaseAudit" /> (for domain entities)
/// and <see cref="ApplicationUser" /> (which inherits IdentityUser instead)
/// </summary>
public interface IAuditable
{
    DateTimeOffset CreatedAt { get; }
    DateTimeOffset UpdatedAt { get; }
    DateTimeOffset? DeletedAt { get; }
}
