using ArchitectPortfolioPlatform.Domain.Common;
using ArchitectPortfolioPlatform.Domain.Portfolio.Articles;
using ArchitectPortfolioPlatform.Domain.Portfolio.Articles.Entities;
using ArchitectPortfolioPlatform.Domain.Portfolio.Articles.Events;

using FluentAssertions;

namespace Backend.Tests.Portfolio.Articles;

public class ArticlesTests
{
    private static Article CreateValidArticle()
    {
        return Article.Create(
            title: "Modular Monoliths in Practice",
            description: "A deep dive into modular monolith architecture.");
    }

    [Fact]
    public void Create_ShouldFail_WhenTitleIsEmpty()
    {
        var action = () => Article.Create(
            "",
            "Description");

        action.Should()
            .Throw<DomainException>()
            .WithMessage("Article title cannot be empty.");
    }

    [Fact]
    public void Create_ShouldCreateDraftArticle()
    {
        var article = CreateValidArticle();

        article.Status.Should()
            .Be(ArticleStatus.Draft);

        article.Tags.Should()
            .BeEmpty();

        article.PublishedDate.Should()
            .BeNull();
    }

    [Fact]
    public void AddTag_ShouldAddTagToArticle()
    {
        var article = CreateValidArticle();

        article.AddTag("architecture");

        article.Tags.Should()
            .ContainSingle()
            .Which.Should()
            .Be("architecture");
    }

    [Fact]
    public void AddTag_ShouldNotAddDuplicateTag()
    {
        var article = CreateValidArticle();

        article.AddTag("architecture");
        article.AddTag("architecture");

        article.Tags.Should()
            .ContainSingle();
    }

    [Fact]
    public void AddTag_ShouldFail_WhenTagIsEmpty()
    {
        var article = CreateValidArticle();

        var action = () => article.AddTag("");

        action.Should()
            .Throw<DomainException>()
            .WithMessage("Tag cannot be empty.");
    }

    [Fact]
    public void Publish_ShouldFail_WhenExternalPlatformHasNoUrl()
    {
        var article = Article.Create(
            title: "Modular Monoliths in Practice",
            description: "A deep dive into modular monolith architecture.",
            platform: "Medium",
            externalUrl: null);

        var action = () => article.Publish();

        action.Should()
            .Throw<DomainException>()
            .WithMessage("A published article hosted on an external platform must have a valid external URL.");
    }

    [Fact]
    public void Publish_ShouldPublish_WhenArticleIsValid()
    {
        var article = CreateValidArticle();

        article.Publish();

        article.Status.Should()
            .Be(ArticleStatus.Published);

        article.PublishedDate.Should()
            .NotBeNull();
    }

    [Fact]
    public void Publish_ShouldPublish_WhenExternalUrlIsProvided()
    {
        var article = Article.Create(
            title: "Modular Monoliths in Practice",
            description: "A deep dive into modular monolith architecture.",
            platform: "Medium",
            externalUrl: "https://medium.com/article");

        article.Publish();

        article.Status.Should()
            .Be(ArticleStatus.Published);
    }

    [Fact]
    public void Publish_ShouldRaiseArticlePublishedEvent()
    {
        var article = CreateValidArticle();

        article.Publish();

        article.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ArticlePublished>();
    }

    [Fact]
    public void Unpublish_ShouldReturnArticleToDraft()
    {
        var article = CreateValidArticle();

        article.Publish();
        article.Unpublish();

        article.Status.Should()
            .Be(ArticleStatus.Draft);

        article.PublishedDate.Should()
            .BeNull();
    }
}
