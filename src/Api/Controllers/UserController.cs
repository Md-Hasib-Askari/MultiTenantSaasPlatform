using Api.Models;
using Application.Users.DTOs;
using Application.Users.Interfaces;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UserController(IUserService userService, ITenantContext tenantContext) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var user = await userService.GetByIdAsync(userId, ct);
        return Ok(ApiResponse<UserResponse>.Ok(user));
    }

    [HttpPatch("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateUserRequest dto, CancellationToken ct)
    {
        var userId = User.GetUserId();
        var user = await userService.UpdateAsync(userId, dto, ct);
        return Ok(ApiResponse<UserResponse>.Ok(user, "User updated successfully"));
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe(CancellationToken ct)
    {
        var userId = User.GetUserId();
        await userService.DeleteAsync(userId, ct);
        return Ok(ApiResponse<object>.Ok("User deleted successfully"));
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers(CancellationToken ct)
    {
        var users = await userService.GetByTenantIdAsync(tenantContext.TenantId, ct);
        return Ok(ApiResponse<IReadOnlyList<UserResponse>>.Ok(users));
    }
}
