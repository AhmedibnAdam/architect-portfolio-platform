using ArchitectPortfolioPlatform.Domain.Common;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Entities;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Events;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.ValueObjects;
using FluentAssertions;

namespace Backend.Tests.Portfolio.Projects;

public class ProjectTests
{
    [Fact]
    public void Publish_ShouldFail_WhenProjectHasNoImages()
    {
        // Arrange
        var project = CreateValidProject();

        // Act
        var action = () => project.Publish();

        // Assert
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Publish_ShouldRaiseProjectPublishedEvent()
    {
        // Arrange
        var project = CreateValidProject();

        project.AddImage(
            ProjectImage.Create(
                "https://example.com/image.jpg",
                "Main image",
                1));

        // Act
        project.Publish();

        // Assert
        project.DomainEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<ProjectPublished>();
    }

    private static Project CreateValidProject()
    {
        return Project.Create(
            "Sample Project",
            "A sample project description.",
            new ProjectMetadata(
                "Sample Client",
                "Sample Location",
                "Residential"),
            ProjectDuration.Create(
                new DateOnly(2024, 1, 1)));
    }
}
