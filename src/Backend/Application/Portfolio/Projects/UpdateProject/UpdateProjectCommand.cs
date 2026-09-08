

namespace Application.Portfolio.Projects.UpdateProject;

public sealed record UpdateProjectCommand(
    Guid ProjectId,
    string Title,
    string ShortDescription,
    string Description,
    Uri? ProjectUrl,
    DateOnly StartDate,
    DateOnly EndDate);