using Application.Tenants.DTOs;
using Application.Tenants.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

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

        return Ok(tenant);
    }

    [HttpGet("by-slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
    {
        var tenant = await tenantService.GetBySlugAsync(slug, ct);
        if (tenant is null)
            return NotFound(new { error = $"Tenant '{slug}' not found." });

        return Ok(tenant);
    }

    [HttpGet("slug-exists")]
    public async Task<IActionResult> SlugExists([FromQuery] string slug, CancellationToken ct)
    {
        var exists = await tenantService.SlugExistsAsync(slug, ct);
        return Ok(new { slug, exists });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantDto dto, CancellationToken ct)
    {
        try
        {
            await tenantService.AddAsync(dto, ct);
            return Ok();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message, errors = ex.Errors });
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTenantDto dto, CancellationToken ct)
    {
        try
        {
            await tenantService.UpdateAsync(id, dto, ct);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message, errors = ex.Errors });
        }
    }
}
