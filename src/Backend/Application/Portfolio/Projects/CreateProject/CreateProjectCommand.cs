namespace Application.Portfolio.Projects.CreateProject;

public sealed record CreateProjectCommand(
    string Title,
    string ShortDescription,
    string Description,
    Uri? ProjectUrl,
    DateOnly StartDate,
    DateOnly? EndDate);

