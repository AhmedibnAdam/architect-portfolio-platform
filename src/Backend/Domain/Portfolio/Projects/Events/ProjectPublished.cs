using ArchitectPortfolioPlatform.Domain.Common;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.ValueObjects;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects;

namespace ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Events;

public sealed record ProjectPublished(
    ProjectId ProjectId,
    DateTime OccurredOnUtc) : IDomainEvent
{
  public static ProjectPublished Create(ProjectId projectId)
    {
        return new ProjectPublished(
            projectId,
            DateTime.UtcNow);
    }
}
