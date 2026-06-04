namespace Domain.Entities.Common;

public class BaseAudit
{
    public DateTimeOffset CreatedAt { get; protected set; }
    public DateTimeOffset UpdatedAt { get; protected set; }
    public DateTimeOffset? DeletedAt { get; protected set; }
}
