using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.ValueObjects;
using ArchitectPortfolioPlatform.Domain.Common;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Events;


namespace ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Entities;

public sealed class Project
    : AggregateRoot<ProjectId>
{
    private readonly List<ProjectImage> _images = [];

    public string Title { get; private set; }

    public string Description { get; private set; }

    public ProjectMetadata Metadata { get; private set; }

    public ProjectDuration Duration { get; private set; }

    public ProjectStatus Status { get; private set; }

    public bool IsFeatured { get; private set; }

    public IReadOnlyCollection<ProjectImage> Images =>
        _images.AsReadOnly();

    private Project()
        : base(default)
    {
        Title = string.Empty;
        Description = string.Empty;
        Metadata = null!;
        Duration = null!;
    }

    private Project(
        ProjectId id,
        string title,
        string description,
        ProjectMetadata metadata,
        ProjectDuration duration)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException(
                "Project title cannot be empty.");
        }

        Title = title;
        Description = description;
        Metadata = metadata;
        Duration = duration;
        Status = ProjectStatus.Draft;
    }

    public static Project Create(
        string title,
        string description,
        ProjectMetadata metadata,
        ProjectDuration duration)
    {
        return new Project(
            ProjectId.New(),
            title,
            description,
            metadata,
            duration);
    }

    public void AddImage(ProjectImage image)
    {
        ArgumentNullException.ThrowIfNull(image);

        _images.Add(image);
    }
    public void MarkAsFeatured()
    {
        if (!_images.Any(x => x.IsHero))
        {
            throw new DomainException(
                "A featured project must have a hero image.");
        }

        IsFeatured = true;
    }

    public void Publish()
{
    if (string.IsNullOrWhiteSpace(Title))
    {
        throw new DomainException(
            "A project must have a title before publishing.");
    }

    if (!_images.Any())
    {
        throw new DomainException(
            "A published project must have a thumbnail image.");
    }

    if (IsFeatured && !_images.Any(x => x.IsHero))
    {
        throw new DomainException(
            "A featured project must have a hero image.");
    }

    Status = ProjectStatus.Published;

    AddDomainEvent(
        new ProjectPublished(Id, DateTime.UtcNow));
}

public void Unpublish()
{
    Status = ProjectStatus.Draft;
}
}