

using Application.Common.Interfaces;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.ValueObjects;

namespace Application.Portfolio.Projects.UpdateProject;

public sealed class UpdateProjectHandler: ICommandHandler<UpdateProjectCommand, UpdateProjectResult> 
{
    private readonly IProjectRepository _projectRepository;

    public UpdateProjectHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<UpdateProjectResult> Handle(UpdateProjectCommand command, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(
            new ProjectId(command.ProjectId),
            cancellationToken);

        if (project is null)
        {
            throw new InvalidOperationException($"Project with ID {command.ProjectId} not found.");
        }

        var metadata = ProjectMetadata.Create(
            title: command.Title,
            shortDescription: command.ShortDescription,
            description: command.Description,
            projectUrl: command.ProjectUrl);

        var duration = ProjectDuration.Create(
            command.StartDate,
            command.EndDate);

        project.Update(
            title: command.Title,
            description: command.Description,
            metadata: metadata,
            duration: duration);

        await _projectRepository.UpdateAsync(project, cancellationToken);

        return new UpdateProjectResult(project.Id.Value);
    }
}
 