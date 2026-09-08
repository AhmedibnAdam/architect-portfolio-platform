

namespace Api.Projects.CreateProject;

public sealed record CreateProjectRequest(
    string Title,
    string ShortDescription,
    string Description,
    string? ProjectUrl,
    DateOnly StartDate,
    DateOnly? EndDate);