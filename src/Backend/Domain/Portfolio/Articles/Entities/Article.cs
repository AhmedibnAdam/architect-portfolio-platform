
using ArchitectPortfolioPlatform.Domain.Common;
using ArchitectPortfolioPlatform.Domain.Portfolio.Articles.Events;
using ArchitectPortfolioPlatform.Domain.Portfolio.Articles.ValueObjects;

namespace ArchitectPortfolioPlatform.Domain.Portfolio.Articles.Entities;

public sealed class Article
    : AggregateRoot<ArticleId>
{
    private readonly List<string> _tags = [];

    public string Title { get; private set; }

    public string Description { get; private set; }

    public string? Platform { get; private set; }

    public string? ExternalUrl { get; private set; }

    public DateOnly? PublishedDate { get; private set; }

    public ArticleStatus Status { get; private set; }

    public IReadOnlyCollection<string> Tags =>
        _tags.AsReadOnly();

    private Article() : base(default)
    {
        Title = string.Empty;
        Description = string.Empty;
    }

    private Article(
        ArticleId id,
        string title,
        string description,
        string? platform,
        string? externalUrl) : base(id)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException(
                "Article title cannot be empty.");
        }

        Title = title;
        Description = description;
        Platform = platform;
        ExternalUrl = externalUrl;
        Status = ArticleStatus.Draft;
    }

    public static Article Create(
        string title,
        string description,
        string? platform = null,
        string? externalUrl = null)
    {
        return new Article(
            ArticleId.New(),
            title,
            description,
            platform,
            externalUrl);
    }

    public void AddTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new DomainException(
                "Tag cannot be empty.");
        }

        if (!_tags.Contains(tag))
        {
            _tags.Add(tag);
        }
    }

    public void Publish()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            throw new DomainException(
                "An article must have a title before publishing.");
        }

        if (!string.IsNullOrWhiteSpace(Platform) && string.IsNullOrWhiteSpace(ExternalUrl))
        {
            throw new DomainException(
                "A published article hosted on an external platform must have a valid external URL.");
        }

        Status = ArticleStatus.Published;
        PublishedDate = DateOnly.FromDateTime(DateTime.UtcNow);

        AddDomainEvent(
            ArticlePublished.Create(Id));
    }

    public void Unpublish()
    {
        Status = ArticleStatus.Draft;
        PublishedDate = null;
    }
}
