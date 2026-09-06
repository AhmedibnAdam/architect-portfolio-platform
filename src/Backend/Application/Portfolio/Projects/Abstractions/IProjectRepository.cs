
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Entities;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.ValueObjects;

namespace Application.Common.Interfaces;

public interface IProjectRepository
{
    Task<Project> GetByIdAsync(ProjectId projectId, CancellationToken cancellationToken);
    Task AddAsync(Project project, CancellationToken cancellationToken);
    Task UpdateAsync(Project project, CancellationToken cancellationToken);
    Task RemoveAsync(Project project, CancellationToken cancellationToken);
}