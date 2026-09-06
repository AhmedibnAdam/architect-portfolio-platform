
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Entities;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.ValueObjects;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects;
using Application.Common.Interfaces;

namespace Application.Portfolio.Projects.CreateProject;

public sealed class CreateProjectHandler
    : ICommandHandler<CreateProjectCommand, CreateProjectResult>
{
    private readonly IProjectRepository _projectRepository;

    public CreateProjectHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<CreateProjectResult> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        var metadata = ProjectMetadata.Create(title: command.Title,
        shortDescription: command.ShortDescription,
            description: command.Description,
            projectUrl: command.ProjectUrl);

        var duration = ProjectDuration.Create(
            command.StartDate,
            command.EndDate);

        var project = Project.Create(
            title: command.Title,
            description: command.Description,
            metadata: metadata,
            duration: duration);

        await _projectRepository.AddAsync(project, cancellationToken);

        return new CreateProjectResult(project.Id.Value);
    }
}