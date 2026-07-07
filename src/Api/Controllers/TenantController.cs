using Api.Models;
using Application.Tenants.DTOs;
using Application.Tenants.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/tenants")]
public class TenantController(ITenantService tenantService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var tenant = await tenantService.GetByIdAsync(id, ct);
        if (tenant is null)
            return NotFound(new { error = $"Tenant with ID {id} not found." });

        return Ok(ApiResponse<object>.Ok(tenant));
    }

    [HttpGet("by-slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
    {
        var tenant = await tenantService.GetBySlugAsync(slug, ct);
        if (tenant is null)
            return NotFound(new { error = $"Tenant '{slug}' not found." });

        return Ok(ApiResponse<object>.Ok(tenant));
    }

    [HttpGet("slug-exists")]
    public async Task<IActionResult> SlugExists([FromQuery] string slug, CancellationToken ct)
    {
        var exists = await tenantService.SlugExistsAsync(slug, ct);
        return Ok(ApiResponse<object>.Ok(new { slug, exists }));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTenantRequest dto,
        CancellationToken ct
    )
    {
        var userId = ClaimsPrincipalExtensions.GetUserId(User);

        await tenantService.AddAsync(dto, userId, ct);
        return Ok(ApiResponse<object>.Ok("Tenant created successfully"));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTenantRequest dto,
        CancellationToken ct
    )
    {
        var userId = ClaimsPrincipalExtensions.GetUserId(User);

        await tenantService.UpdateAsync(id, dto, userId, ct);
        return Ok(ApiResponse<object>.Ok("Tenant updated successfully"));
    }
}
