
using ArchitectPortfolioPlatform.Domain.Common;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.ValueObjects;

namespace ArchitectPortfolioPlatform.Domain.Portfolio.Projects.ValueObjects;

public sealed record ProjectDuration
{
    public DateOnly StartDate { get; }
    public DateOnly? EndDate { get; }

    private ProjectDuration(
        DateOnly startDate,
        DateOnly? endDate)
    {
        if (endDate.HasValue && endDate.Value < startDate)
        {
            throw new DomainException(
                "Project end date cannot precede start date.");
        }

        StartDate = startDate;
        EndDate = endDate;
    }

    public static ProjectDuration Create(
        DateOnly startDate,
        DateOnly? endDate = null)
    {
        return new ProjectDuration(startDate, endDate);
    }

    public bool IsOngoing => !EndDate.HasValue;
}