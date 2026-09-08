

namespace Api.Projects.updateProject;

public sealed record UpdateProjectRequest
(
    string Title,
    string ShortDescription,
    string Description,
    string? ProjectUrl,
    DateOnly StartDate,
    DateOnly EndDate
);