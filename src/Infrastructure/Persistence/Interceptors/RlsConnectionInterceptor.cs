using System.Data.Common;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

public class RlsConnectionInterceptor(ITenantContext tenantContext) : DbConnectionInterceptor
{
    private readonly ITenantContext _tenantContext = tenantContext;

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        base.ConnectionOpened(connection, eventData);
        SetTenantId(connection);
    }

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default
    )
    {
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
        await SetTenantIdAsync(connection, cancellationToken);
    }

    private static string BuildSetTenantIdSql(ITenantContext tenantContext) =>
        tenantContext.IsResolved && tenantContext.TenantId != Guid.Empty
            ? $"SET app.tenant_id = '{tenantContext.TenantId}'"
            : "SET app.tenant_id = '00000000-0000-00000-0000-000000000000'";

    private void SetTenantId(DbConnection conn)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = BuildSetTenantIdSql(_tenantContext);
        cmd.ExecuteNonQuery();
    }

    private async Task SetTenantIdAsync(DbConnection conn, CancellationToken ct)
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = BuildSetTenantIdSql(_tenantContext);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
