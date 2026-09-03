namespace ArchitectPortfolioPlatform.Domain.Portfolio.Profile.ValueObjects;

public readonly record struct ProfileId(Guid Value)
{
    public static ProfileId New() => new(Guid.NewGuid());
}
