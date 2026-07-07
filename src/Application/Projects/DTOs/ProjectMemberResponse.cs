using Domain.Entities.Projects;

namespace Application.Projects.DTOs;

public record ProjectMemberResponse(Guid UserId, ProjectMemberRole Role);
