namespace ArchitectPortfolioPlatform.Domain.Portfolio.Projects.ValueObjects;

public sealed record ProjectMetadata
{
    public string Title { get; }
    public string ShortDescription { get; }
    public string Description { get; }
    public Uri? ProjectUrl { get; }

    private ProjectMetadata(
        string title,
        string shortDescription,
        string description,
        Uri? projectUrl)
    {
        Title = title;
        ShortDescription = shortDescription;
        Description = description;
        ProjectUrl = projectUrl;
    }

    public static ProjectMetadata Create(
        string title,
        string shortDescription,
        string description,
        Uri? projectUrl = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Project title is required.",
                nameof(title));

        if (title.Length > 200)
            throw new ArgumentException(
                "Project title cannot exceed 200 characters.",
                nameof(title));

        if (string.IsNullOrWhiteSpace(shortDescription))
            throw new ArgumentException(
                "Project short description is required.",
                nameof(shortDescription));

        if (shortDescription.Length > 500)
            throw new ArgumentException(
                "Project short description cannot exceed 500 characters.",
                nameof(shortDescription));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Project description is required.",
                nameof(description));

        return new ProjectMetadata(
            title.Trim(),
            shortDescription.Trim(),
            description.Trim(),
            projectUrl);
    }
}