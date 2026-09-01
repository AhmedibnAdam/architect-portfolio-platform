

namespace ArchitectPortfolioPlatform.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}