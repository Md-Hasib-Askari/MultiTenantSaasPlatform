namespace Domain.Entities.Common;

public interface ICreateAudit
{
    DateTimeOffset CreatedAt { get; }

    Guid CreatedById { get; }
    ApplicationUser? CreatedBy { get; }
}
