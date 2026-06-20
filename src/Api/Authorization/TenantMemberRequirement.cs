using Microsoft.AspNetCore.Authorization;

namespace Api.Authorization;

public class TenantMemberRequirement(params string[] allowedRoles) : IAuthorizationRequirement
{
    public string[] AllowedRoles { get; } = allowedRoles;
}
