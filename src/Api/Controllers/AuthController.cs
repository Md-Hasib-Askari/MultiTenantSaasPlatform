using Application.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private const string RefreshTokenCookie = "refresh_token";

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken ct
    )
    {
        var result = await _authService.RegisterAsync(request, ct);
        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(new { accessToken = result.AccessToken });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct
    )
    {
        var result = await _authService.LoginAsync(request, ct);
        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(new { accessToken = result.AccessToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookie];
        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest(new { error = "No refresh token provided." });

        var result = await _authService.RefreshAsync(refreshToken, ct);
        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(new { accessToken = result.AccessToken });
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append(RefreshTokenCookie, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            IsEssential = true,
        });
    }
}
