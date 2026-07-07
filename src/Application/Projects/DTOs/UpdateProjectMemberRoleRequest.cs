using System.ComponentModel.DataAnnotations;
using Domain.Entities.Projects;

namespace Application.Projects.DTOs;

public record UpdateProjectMemberRoleRequest([Required] ProjectMemberRole Role);
