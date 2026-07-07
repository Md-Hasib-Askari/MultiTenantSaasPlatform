using System.ComponentModel.DataAnnotations;

namespace Application.Projects.DTOs;

public record AddProjectMemberRequest([Required] Guid UserId);
