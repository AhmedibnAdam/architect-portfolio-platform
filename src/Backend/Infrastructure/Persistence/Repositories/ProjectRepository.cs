using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Entities;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.ValueObjects;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Infrastructure.persistence;


namespace Infrastructure.Persistence.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly PortfolioDbContext _dbContext;

    public ProjectRepository(PortfolioDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Project project, CancellationToken cancellationToken)
    {
        await _dbContext.Projects.AddAsync(project, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Project project, CancellationToken cancellationToken)
    {
        _dbContext.Projects.Update(project);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Project project, CancellationToken cancellationToken)
    {
        _dbContext.Projects.Remove(project);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Project> GetByIdAsync(ProjectId projectId, CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects.FindAsync([projectId], cancellationToken);

        return project ?? throw new NotFoundException(nameof(Project), projectId.Value);
    }
}
