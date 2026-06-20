using Microsoft.AspNetCore.Authorization;

namespace Api.Authorization;

public class TenantRoleRequirement(params string[] allowedRoles) : IAuthorizationRequirement
{
    public string[] AllowedRoles { get; } = allowedRoles;
}
