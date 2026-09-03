namespace ArchitectPortfolioPlatform.Domain.Portfolio.Skills.ValueObjects;

public readonly record struct SkillId(Guid Value)
{
    public static SkillId New() => new(Guid.NewGuid());
}
