using System.ComponentModel.DataAnnotations;

namespace Application.Projects.DTOs;

public record CreateProjectRequest([Required] string Name, string? Description, string? Color);
