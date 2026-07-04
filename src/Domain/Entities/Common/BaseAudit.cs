namespace Domain.Entities.Common;

public class BaseAudit : ICreateAudit, IUpdateAudit, IDeleteAudit
{
    public DateTimeOffset CreatedAt { get; protected set; }

    public Guid CreatedById { get; protected set; }
    public ApplicationUser CreatedBy { get; protected set; } = null!;

    public DateTimeOffset? UpdatedAt { get; protected set; }

    public Guid? UpdatedById { get; protected set; }
    public ApplicationUser? UpdatedBy { get; protected set; }

    public DateTimeOffset? DeletedAt { get; protected set; }

    public Guid? DeletedById { get; protected set; }
    public ApplicationUser? DeletedBy { get; protected set; }
}
