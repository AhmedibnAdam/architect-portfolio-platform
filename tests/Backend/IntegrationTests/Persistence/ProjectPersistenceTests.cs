using ArchitectPortfolioPlatform.Domain.Portfolio.Projects;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Entities;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Backend.IntegrationTests.Persistence;

[Collection(nameof(PostgresCollection))]
public sealed class ProjectPersistenceTests
{
    private readonly PostgresDatabaseFixture _fixture;

    public ProjectPersistenceTests(PostgresDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    private static Project CreateProject(
        string title = "Portfolio Platform",
        DateOnly? start = null,
        DateOnly? end = null)
    {
        var metadata = ProjectMetadata.Create(
            title,
            "A short description",
            "A much longer, detailed description of the project.",
            new Uri("https://example.com/project"));

        var duration = ProjectDuration.Create(
            start ?? new DateOnly(2024, 1, 1),
            end);

        return Project.Create(
            title,
            "Project description",
            metadata,
            duration);
    }

    [Fact]
    public async Task Project_can_be_persisted()
    {
        var project = CreateProject();

        await using (var context = _fixture.CreateContext())
        {
            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }

        await using (var context = _fixture.CreateContext())
        {
            var exists = await context.Projects.AnyAsync(p => p.Id == project.Id);
            exists.Should().BeTrue();
        }
    }

    [Fact]
    public async Task ProjectId_round_trips_through_its_value_converter()
    {
        var project = CreateProject();

        await using (var context = _fixture.CreateContext())
        {
            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }

        await using (var context = _fixture.CreateContext())
        {
            var loaded = await context.Projects
                .SingleAsync(p => p.Id == project.Id);

            loaded.Id.Should().Be(project.Id);
            loaded.Id.Value.Should().Be(project.Id.Value);
        }
    }

    [Fact]
    public async Task ProjectMetadata_is_persisted_correctly()
    {
        var project = CreateProject(
            title: "Metadata Test Project");

        await using (var context = _fixture.CreateContext())
        {
            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }

        await using (var context = _fixture.CreateContext())
        {
            var loaded = await context.Projects
                .SingleAsync(p => p.Id == project.Id);

            loaded.Metadata.Should().NotBeNull();
            loaded.Metadata.Title.Should().Be(project.Metadata.Title);
            loaded.Metadata.ShortDescription.Should().Be(project.Metadata.ShortDescription);
            loaded.Metadata.Description.Should().Be(project.Metadata.Description);
            loaded.Metadata.ProjectUrl.Should().Be(project.Metadata.ProjectUrl);
        }
    }

    [Fact]
    public async Task ProjectDuration_is_persisted_correctly()
    {
        var start = new DateOnly(2023, 6, 1);
        var end = new DateOnly(2023, 12, 31);
        var project = CreateProject(start: start, end: end);

        await using (var context = _fixture.CreateContext())
        {
            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }

        await using (var context = _fixture.CreateContext())
        {
            var loaded = await context.Projects
                .SingleAsync(p => p.Id == project.Id);

            loaded.Duration.StartDate.Should().Be(start);
            loaded.Duration.EndDate.Should().Be(end);
            loaded.Duration.IsOngoing.Should().BeFalse();
        }
    }

    [Fact]
    public async Task ProjectDuration_with_no_end_date_is_persisted_as_ongoing()
    {
        var project = CreateProject(end: null);

        await using (var context = _fixture.CreateContext())
        {
            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }

        await using (var context = _fixture.CreateContext())
        {
            var loaded = await context.Projects
                .SingleAsync(p => p.Id == project.Id);

            loaded.Duration.EndDate.Should().BeNull();
            loaded.Duration.IsOngoing.Should().BeTrue();
        }
    }

    [Fact]
    public async Task ProjectImage_relationship_is_persisted_and_cascade_deletes()
    {
        var project = CreateProject();
        var heroImage = ProjectImage.Create("https://example.com/hero.png", "Hero", 0, isHero: true);
        var galleryImage = ProjectImage.Create("https://example.com/gallery.png", "Gallery", 1);

        project.AddImage(heroImage);
        project.AddImage(galleryImage);

        await using (var context = _fixture.CreateContext())
        {
            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }

        await using (var context = _fixture.CreateContext())
        {
            var loaded = await context.Projects
                .Include(p => p.Images)
                .SingleAsync(p => p.Id == project.Id);

            loaded.Images.Should().HaveCount(2);
            loaded.Images.Should().Contain(i => i.Url == "https://example.com/hero.png" && i.IsHero);
            loaded.Images.Should().Contain(i => i.Url == "https://example.com/gallery.png" && !i.IsHero);
        }

        await using (var context = _fixture.CreateContext())
        {
            var entity = await context.Projects.SingleAsync(p => p.Id == project.Id);
            context.Projects.Remove(entity);
            await context.SaveChangesAsync();
        }

        await using (var context = _fixture.CreateContext())
        {
            var remainingImages = await context.Set<ProjectImage>()
                .Where(i => i.Id == heroImage.Id || i.Id == galleryImage.Id)
                .ToListAsync();

            remainingImages.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task Project_can_be_loaded_back_into_the_domain_model()
    {
        var project = CreateProject(title: "Round Trip Project");
        var image = ProjectImage.Create("https://example.com/image.png", "Caption", 0, isHero: true);
        project.AddImage(image);
        project.MarkAsFeatured();
        project.Publish();

        await using (var context = _fixture.CreateContext())
        {
            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }

        await using (var context = _fixture.CreateContext())
        {
            var loaded = await context.Projects
                .Include(p => p.Images)
                .SingleAsync(p => p.Id == project.Id);

            loaded.Should().BeOfType<Project>();
            loaded.Title.Should().Be(project.Title);
            loaded.Description.Should().Be(project.Description);
            loaded.Status.Should().Be(ProjectStatus.Published);
            loaded.IsFeatured.Should().BeTrue();
            loaded.Images.Should().ContainSingle(i => i.IsHero);
        }
    }
}
