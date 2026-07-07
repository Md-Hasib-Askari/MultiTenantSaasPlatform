using Application.Auth;
using Application.Common;
using Application.Tenants.Interfaces;
using Application.Users.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class AuthService(
    IUserRepository userRepo,
    IPasswordHasher<ApplicationUser> passwordHasher,
    ITokenService tokenService,
    ITenantService tenantService,
    IRefreshTokenRepository refreshTokenRepo,
    IUserTenantRoleRepository userTenantRoleRepo,
    AppDbContext db
) : IAuthService
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher = passwordHasher;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ITenantService _tenantService = tenantService;
    private readonly IRefreshTokenRepository _refreshTokenRepo = refreshTokenRepo;
    private readonly IUserTenantRoleRepository _userTenantRoleRepo = userTenantRoleRepo;
    private readonly AppDbContext _db = db;

    public async Task<TokenResponse> LoginAsync(
        LoginRequest request,
        CancellationToken ct = default
    )
    {
        var user =
            await _userRepo.GetByEmailAsync(request.Email, ct)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (user.Status != UserStatus.Active)
            throw new UnauthorizedAccessException("Account is not active.");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result is PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var tenant =
            await _tenantService.GetByIdAsync(user.PrimaryTenantId, ct)
            ?? throw new InvalidOperationException("Primary Tenant not found.");

        var userRole = await _userTenantRoleRepo.GetByUserAndTenantAsync(user.Id, tenant.Id, ct);
        var role = userRole?.Role ?? TenantRole.Member;

        var (accessToken, refreshToken) = _tokenService.CreateTokenPair(user, tenant, role);

        var tokenHash = _tokenService.HashToken(refreshToken);
        var refreshEntity = RefreshToken.Create(user.Id, tokenHash, expiryDays: 7);
        await _refreshTokenRepo.AddAsync(refreshEntity, ct);
        await _db.SaveChangesAsync(ct);

        return new TokenResponse(accessToken, refreshToken);
    }

    public async Task<TokenResponse> RefreshAsync(
        string refreshToken,
        CancellationToken ct = default
    )
    {
        var tokenHash = _tokenService.HashToken(refreshToken);

        var storedToken = await _refreshTokenRepo.GetByHashWithUserAsync(tokenHash, ct);
        if (storedToken is null || !storedToken.IsActive)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        storedToken.Revoke();

        var user = storedToken.User;
        if (user.Status != UserStatus.Active)
            throw new UnauthorizedAccessException("Account is not active.");

        var tenant =
            await _tenantService.GetByIdAsync(user.PrimaryTenantId, ct)
            ?? throw new InvalidOperationException("Primary tenant not found.");

        var userRole = await _userTenantRoleRepo.GetByUserAndTenantAsync(user.Id, tenant.Id, ct);
        var role = userRole?.Role ?? TenantRole.Member;

        var (accessToken, newRefreshToken) = _tokenService.CreateTokenPair(user, tenant, role);

        var newTokenHash = _tokenService.HashToken(newRefreshToken);
        var newRefreshEntity = RefreshToken.Create(user.Id, newTokenHash, expiryDays: 7);
        await _refreshTokenRepo.AddAsync(newRefreshEntity, ct);
        await _db.SaveChangesAsync(ct);

        return new TokenResponse(accessToken, newRefreshToken);
    }

    public async Task<TokenResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken ct = default
    )
    {
        var tenant =
            await _tenantService.GetBySlugAsync(request.TenantSlug, ct)
            ?? throw new InvalidOperationException("Tenant not found.");

        if (tenant.Status != TenantStatus.Active)
            throw new InvalidOperationException("Tenant is not active.");

        var existing = await _userRepo.GetByEmailAsync(request.Email, ct);
        if (existing is not null)
            throw new InvalidOperationException("A user with this email already exists.");

        var user = ApplicationUser.Create(
            userName: request.Email,
            email: request.Email,
            displayName: request.DisplayName,
            primaryTenantId: tenant.Id
        );

        user.SetPasswordHash(_passwordHasher.HashPassword(user, request.Password));
        await _userRepo.AddAsync(user, ct);

        var userTenantRole = UserTenantRole.Create(user.Id, tenant.Id, TenantRole.Member);
        await _userTenantRoleRepo.AddAsync(userTenantRole, ct);
        await _db.SaveChangesAsync(ct);

        var (accessToken, refreshToken) = _tokenService.CreateTokenPair(
            user,
            tenant,
            TenantRole.Member
        );

        var tokenHash = _tokenService.HashToken(refreshToken);
        var refreshEntity = RefreshToken.Create(user.Id, tokenHash, expiryDays: 7);
        await _refreshTokenRepo.AddAsync(refreshEntity, ct);
        await _db.SaveChangesAsync(ct);

        return new TokenResponse(accessToken, refreshToken);
    }
}
