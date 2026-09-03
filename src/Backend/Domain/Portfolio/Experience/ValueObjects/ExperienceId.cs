namespace ArchitectPortfolioPlatform.Domain.Portfolio.Experience.ValueObjects;

public readonly record struct ExperienceId(Guid Value)
{
    public static ExperienceId New() => new(Guid.NewGuid());
}
