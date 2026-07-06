using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Common;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Identity;

public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string PrivateKey { get; set; } = string.Empty; // PEM
    public string PublicKey { get; set; } = string.Empty; // PEM
    public int ExpiryHour { get; set; } = 1;
}

public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _opts;
    private readonly RsaSecurityKey _signingKey;

    public JwtTokenService(IOptions<JwtOptions> opts, RSA rsa)
    {
        _opts = opts.Value;
        _signingKey = new RsaSecurityKey(rsa);
    }

    public (string AccessToken, string RefreshToken) CreateTokenPair(
        ApplicationUser user,
        Tenant tenant,
        string role
    )
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("email", user.Email!),
            new Claim("tenant_id", tenant.Id.ToString()),
            new Claim("tenant_slug", tenant.Slug.ToString()),
            new Claim("tenant_role", role),
            new Claim("plan", tenant.Plan.ToString()),
        };

        var creds = new SigningCredentials(_signingKey, SecurityAlgorithms.RsaSha256);
        var token = new JwtSecurityToken(
            issuer: _opts.Issuer,
            audience: _opts.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(_opts.ExpiryHour),
            signingCredentials: creds
        );

        var access = new JwtSecurityTokenHandler().WriteToken(token);
        var refresh = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return (access, refresh);
    }

    public string HashToken(string rawToken)
    {
        var pepper = Convert.FromBase64String(
            Environment.GetEnvironmentVariable("TOKEN_PEPPER")
                ?? "MTNQZXBwZXJTZWVkS2V5Rm9yRGV2T25seQ=="
        );
        using var hmac = new HMACSHA256(pepper);
        return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(rawToken)));
    }
}
