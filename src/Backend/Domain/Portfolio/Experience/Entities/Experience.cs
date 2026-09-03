// Experience
//  ├── Company
//  ├── Position
//  ├── Description
//  ├── Responsibilities
//  ├── Achievements
//  ├── StartDate
//  ├── EndDate
//  └── IsCurrent
// ```

using ArchitectPortfolioPlatform.Domain.Common;
using ArchitectPortfolioPlatform.Domain.Portfolio.Experience.ValueObjects;

namespace ArchitectPortfolioPlatform.Domain.Portfolio.Experience.Entities;

public sealed class Experience
    : AggregateRoot<ExperienceId>
{
    private readonly List<string> _responsibilities = [];
    private readonly List<string> _achievements = [];

    public string Company { get; private set; }

    public string Position { get; private set; }

    public string? Description { get; private set; }

    public DateOnly StartDate { get; private set; }

    public DateOnly? EndDate { get; private set; }

    public bool IsCurrent { get; private set; }

    public IReadOnlyCollection<string> Responsibilities =>
        _responsibilities.AsReadOnly();

    public IReadOnlyCollection<string> Achievements =>
        _achievements.AsReadOnly();

    private Experience() : base(default)
    {
        Company = string.Empty;
        Position = string.Empty;
    }

    private Experience(
        ExperienceId id,
        string company,
        string position,
        string? description,
        DateOnly startDate,
        DateOnly? endDate) : base(id)
    {
        if (string.IsNullOrWhiteSpace(company))
        {
            throw new DomainException(
                "Experience company cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(position))
        {
            throw new DomainException(
                "Experience position cannot be empty.");
        }

        if (endDate.HasValue && endDate.Value < startDate)
        {
            throw new DomainException(
                "Experience end date cannot precede start date.");
        }

        Company = company;
        Position = position;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        IsCurrent = !endDate.HasValue;
    }

    public static Experience Create(
        string company,
        string position,
        string? description,
        DateOnly startDate,
        DateOnly? endDate = null)
    {
        return new Experience(
            ExperienceId.New(),
            company,
            position,
            description,
            startDate,
            endDate);
    }

    public void AddResponsibility(string responsibility)
    {
        if (string.IsNullOrWhiteSpace(responsibility))
        {
            throw new DomainException(
                "Responsibility cannot be empty.");
        }

        if (!_responsibilities.Contains(responsibility))
        {
            _responsibilities.Add(responsibility);
        }
    }

    public void AddAchievement(string achievement)
    {
        if (string.IsNullOrWhiteSpace(achievement))
        {
            throw new DomainException(
                "Achievement cannot be empty.");
        }

        if (!_achievements.Contains(achievement))
        {
            _achievements.Add(achievement);
        }
    }

    public void EndRole(DateOnly endDate)
    {
        if (endDate < StartDate)
        {
            throw new DomainException(
                "Experience end date cannot precede start date.");
        }

        EndDate = endDate;
        IsCurrent = false;
    }
}
