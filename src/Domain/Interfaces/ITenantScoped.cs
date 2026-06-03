namespace Domain.Interfaces;

public interface ITenantScoped
{
    Guid TenantId { get; }
}
