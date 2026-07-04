namespace Domain.Entities.Common;

public interface IUpdateAudit
{
    DateTimeOffset? UpdatedAt { get; }

    Guid? UpdatedById { get; }
    ApplicationUser? UpdatedBy { get; }
}
