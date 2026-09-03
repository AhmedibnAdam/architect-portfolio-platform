using ArchitectPortfolioPlatform.Domain.Common;
using ArchitectPortfolioPlatform.Domain.Portfolio.Articles.ValueObjects;

namespace ArchitectPortfolioPlatform.Domain.Portfolio.Articles.Events;

public sealed record ArticlePublished(
    ArticleId ArticleId,
    DateTime OccurredOnUtc) : IDomainEvent
{
    public static ArticlePublished Create(ArticleId articleId)
    {
        return new ArticlePublished(
            articleId,
            DateTime.UtcNow);
    }
}
