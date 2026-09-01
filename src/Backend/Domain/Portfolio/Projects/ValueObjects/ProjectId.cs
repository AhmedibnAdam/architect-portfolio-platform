
namespace ArchitectPortfolioPlatform.Domain.Portfolio.Projects.ValueObjects;

public readonly record struct ProjectId(Guid Value)
{
    public static ProjectId New() => new(Guid.NewGuid());
}
