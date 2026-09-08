using Application.Common.Exceptions;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Entities;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.ValueObjects;
using FluentAssertions;
using Infrastructure.Persistence.Repositories;

namespace Backend.IntegrationTests.Persistence;

[Collection(nameof(PostgresCollection))]
public sealed class ProjectRepositoryTests
{
    private readonly PostgresDatabaseFixture _fixture;

    public ProjectRepositoryTests(PostgresDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    private static Project CreateProject(string title = "Repository Test Project")
    {
        var metadata = ProjectMetadata.Create(
            title,
            "A short description",
            "A much longer, detailed description of the project.",
            new Uri("https://example.com/project"));

        var duration = ProjectDuration.Create(new DateOnly(2024, 1, 1), null);

        return Project.Create(title, "Project description", metadata, duration);
    }

    [Fact]
    public async Task AddAsync_persists_the_aggregate()
    {
        var project = CreateProject();

        await using (var context = _fixture.CreateContext())
        {
            var repository = new ProjectRepository(context);
            await repository.AddAsync(project, CancellationToken.None);
        }

        await using (var context = _fixture.CreateContext())
        {
            var repository = new ProjectRepository(context);
            var loaded = await repository.GetByIdAsync(project.Id, CancellationToken.None);

            loaded.Id.Should().Be(project.Id);
            loaded.Title.Should().Be(project.Title);
        }
    }

    [Fact]
    public async Task GetByIdAsync_throws_NotFoundException_when_project_does_not_exist()
    {
        await using var context = _fixture.CreateContext();
        var repository = new ProjectRepository(context);

        var act = () => repository.GetByIdAsync(ProjectId.New(), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_persists_changes_to_an_existing_aggregate()
    {
        var project = CreateProject();

        await using (var context = _fixture.CreateContext())
        {
            var repository = new ProjectRepository(context);
            await repository.AddAsync(project, CancellationToken.None);
        }

        await using (var context = _fixture.CreateContext())
        {
            var repository = new ProjectRepository(context);
            var loaded = await repository.GetByIdAsync(project.Id, CancellationToken.None);
            loaded.Publish();
            loaded.MarkAsFeatured();
            await repository.UpdateAsync(loaded, CancellationToken.None);
        }

        await using (var context = _fixture.CreateContext())
        {
            var repository = new ProjectRepository(context);
            var reloaded = await repository.GetByIdAsync(project.Id, CancellationToken.None);

            reloaded.Status.Should().Be(ProjectStatus.Published);
            reloaded.IsFeatured.Should().BeTrue();
        }
    }

    [Fact]
    public async Task RemoveAsync_deletes_the_aggregate()
    {
        var project = CreateProject();

        await using (var context = _fixture.CreateContext())
        {
            var repository = new ProjectRepository(context);
            await repository.AddAsync(project, CancellationToken.None);
        }

        await using (var context = _fixture.CreateContext())
        {
            var repository = new ProjectRepository(context);
            var loaded = await repository.GetByIdAsync(project.Id, CancellationToken.None);
            await repository.RemoveAsync(loaded, CancellationToken.None);
        }

        await using (var context = _fixture.CreateContext())
        {
            var repository = new ProjectRepository(context);
            var act = () => repository.GetByIdAsync(project.Id, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
