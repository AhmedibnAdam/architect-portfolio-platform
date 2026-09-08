using ArchitectPortfolioPlatform.Domain.Common;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Entities;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Events;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.ValueObjects;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects;

using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace Backend.Tests.Portfolio.Projects;

public class ProjectTests
{
    private static Project CreateValidProject()
    {
        var metadata =  ProjectMetadata.Create(title: "Acme",
         shortDescription: "short description",
         description: "description", projectUrl: null);

        var duration = ProjectDuration.Create(
            new DateOnly(2026, 2, 1),
            new DateOnly(2026, 8, 1));

        return Project.Create(
            title: "Architect Portfolio Platform",
            description: "A portfolio platform demonstrating software architecture.",
            metadata: metadata,
            duration: duration);
    }

    [Fact]
    public void Create_ShouldFail_WhenTitleIsEmpty()
    {
        var metadata = ProjectMetadata.Create(title: "Acme",
         shortDescription: "short description",
         description: "description", projectUrl: null);

        var duration = ProjectDuration.Create(
            new DateOnly(2026, 1, 1));

        var action = () => Project.Create(
            "",
            "Description",
            metadata,
            duration);

        action.Should()
            .Throw<DomainException>()
            .WithMessage("Project title cannot be empty.");
    }

    [Fact]
    public void Create_ShouldFail_WhenEndDatePrecedesStartDate()
    {
        var action = () => ProjectDuration.Create(
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 1, 1));

        action.Should()
            .Throw<DomainException>()
            .WithMessage("Project end date cannot precede start date.");
    }

    [Fact]
    public void Create_ShouldCreateDraftProject()
    {
        var project = CreateValidProject();

        project.Status.Should()
            .Be(ProjectStatus.Draft);

        project.Images.Should()
            .BeEmpty();

        project.IsFeatured.Should()
            .BeFalse();
    }

    [Fact]
    public void AddImage_ShouldAddImageToProject()
    {
        var project = CreateValidProject();

        var image = ProjectImage.Create(
            "https://example.com/project.jpg",
            "Project image",
            1);

        project.AddImage(image);

        project.Images.Should()
            .ContainSingle()
            .Which.Should()
            .Be(image);
    }

    [Fact]
    public void Publish_ShouldFail_WhenProjectHasNoImages()
    {
        var project = CreateValidProject();

        var action = () => project.Publish();

        action.Should()
            .Throw<DomainException>()
            .WithMessage("A published project must have a thumbnail image.");
    }

    [Fact]
    public void Publish_ShouldPublish_WhenProjectIsValid()
    {
        var project = CreateValidProject();

        project.AddImage(
            ProjectImage.Create(
                "https://example.com/project.jpg",
                "Main project image",
                1));

        project.Publish();

        project.Status.Should()
            .Be(ProjectStatus.Published);
    }

    [Fact]
    public void Publish_ShouldRaiseProjectPublishedEvent()
    {
        var project = CreateValidProject();

        project.AddImage(
            ProjectImage.Create(
                "https://example.com/project.jpg",
                "Main project image",
                1));

        project.Publish();

        project.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProjectPublished>();
    }

    [Fact]
    public void MarkAsFeatured_ShouldFail_WhenProjectHasNoHeroImage()
    {
        var project = CreateValidProject();

        project.AddImage(
            ProjectImage.Create(
                "https://example.com/project.jpg",
                "Project image",
                1));

        var action = () => project.MarkAsFeatured();

        action.Should()
            .Throw<DomainException>()
            .WithMessage("A featured project must have a hero image.");
    }

    [Fact]
    public void MarkAsFeatured_ShouldMarkProjectAsFeatured_WhenHeroImageExists()
    {
        var project = CreateValidProject();

        project.AddImage(
            ProjectImage.Create(
                "https://example.com/hero.jpg",
                "Hero image",
                1,
                isHero: true));

        project.MarkAsFeatured();

        project.IsFeatured.Should()
            .BeTrue();
    }

    [Fact]
    public void Unpublish_ShouldReturnProjectToDraft()
    {
        var project = CreateValidProject();

        project.AddImage(
            ProjectImage.Create(
                "https://example.com/project.jpg",
                "Project image",
                1));

        project.Publish();
        project.Unpublish();

        project.Status.Should()
            .Be(ProjectStatus.Draft);
    }
}