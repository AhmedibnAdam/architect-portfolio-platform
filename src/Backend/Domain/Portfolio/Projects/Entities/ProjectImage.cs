using ArchitectPortfolioPlatform.Domain.Common;
namespace ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Entities;

public sealed class ProjectImage
{
    public Guid Id { get; private set; }

    public string Url { get; private set; }

    public string? Caption { get; private set; }

    public int DisplayOrder { get; private set; }

    public bool IsHero { get; private set; }

    private ProjectImage()
    {
        // Required by persistence
        Url = string.Empty;
    }

    private ProjectImage(
        Guid id,
        string url,
        string? caption,
        int displayOrder,
        bool isHero)
    {
        Id = id;
        Url = url;
        Caption = caption;
        DisplayOrder = displayOrder;
        IsHero = isHero;
    }

    public static ProjectImage Create(
        string url,
        string? caption,
        int displayOrder,
        bool isHero = false)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new DomainException(
                "Project image URL cannot be empty.");
        }

        return new ProjectImage(
            Guid.NewGuid(),
            url,
            caption,
            displayOrder,
            isHero);
    }

    public void SetAsHero()
    {
        IsHero = true;
    }

    public void UpdateCaption(string? caption)
    {
        Caption = caption;
    }
}