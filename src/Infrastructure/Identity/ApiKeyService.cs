using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Identity;

public class ApiKeyService
{
    private static readonly byte[] Pepper = Convert.FromBase64String(
        Environment.GetEnvironmentVariable("API_KEY_PEPPER") ?? "QXBpS2V5UGVwcGVyU2VlZEZvckRldg=="
    );

    public static (string PlainKey, string KeyHash, string KeyPrefix) Generate()
    {
        var raw = RandomNumberGenerator.GetBytes(32);
        var plain =
            $"sk_live_{Convert.ToBase64String(raw).Replace('+', '-').Replace('/', '_').TrimEnd('=')}";
        var hash = Hash(plain);
        var prefix = plain[..12];
        return (plain, hash, prefix);
    }

    /// <summary>
    /// Constant-time comparison - prevents timing attacks
    /// </summary>
    public static bool Verify(string plain, string storedHash) =>
        CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(Hash(plain)),
            Encoding.UTF8.GetBytes(storedHash)
        );

    private static string Hash(string key)
    {
        using var hmac = new HMACSHA256(Pepper);
        return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(key)));
    }
}
