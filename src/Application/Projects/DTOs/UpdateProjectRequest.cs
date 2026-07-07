using System.ComponentModel.DataAnnotations;

namespace Application.Projects.DTOs;

public record UpdateProjectRequest(string? Name, string? Description, string? Color) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Name is null && Description is null && Color is null)
        {
            yield return new ValidationResult(
                "At least one field (Name, Description, or Color) must be provided.",
                [nameof(Name), nameof(Description), nameof(Color)]);
        }
    }
}
