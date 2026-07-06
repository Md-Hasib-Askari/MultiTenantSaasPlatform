namespace Domain.Entities.Common;

public interface IDeleteAudit
{
    DateTimeOffset? DeletedAt { get; }

    Guid? DeletedById { get; }
    ApplicationUser? DeletedBy { get; }

    void MarkAsDeleted(Guid deletedBy);
}
