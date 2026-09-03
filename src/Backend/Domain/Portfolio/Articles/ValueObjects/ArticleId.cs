namespace ArchitectPortfolioPlatform.Domain.Portfolio.Articles.ValueObjects;

public readonly record struct ArticleId(Guid Value)
{
    public static ArticleId New() => new(Guid.NewGuid());
}
