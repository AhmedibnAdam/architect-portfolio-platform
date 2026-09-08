using Microsoft.EntityFrameworkCore;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Entities;
using ArchitectPortfolioPlatform.Domain.Portfolio.Skills.Entities;
using ArchitectPortfolioPlatform.Domain.Portfolio.Experience.Entities;

namespace Infrastructure.persistence;

public class PortfolioDbContext : DbContext
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<Skill> Skills => Set<Skill>();

    public DbSet<Experience> Experiences => Set<Experience>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PortfolioDbContext).Assembly);
    }
}