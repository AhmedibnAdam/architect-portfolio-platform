using Application.Common.Interfaces;
using Application.Portfolio.Projects.CreateProject;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Entities;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.ValueObjects;
using FluentAssertions;
using Xunit.Abstractions;
using ArchitectPortfolioPlatform.Domain.Common;

namespace Backend.Tests.Portfolio.Projects;

public class CreateProjectHandlerTests
{
    private readonly ITestOutputHelper _output;

    public CreateProjectHandlerTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private sealed class FakeProjectRepository : IProjectRepository
    {
        public Project? AddedProject { get; private set; }

        public Func<Project, CancellationToken, Task>? OnAddAsync { get; set; }

        public Task<Project> GetByIdAsync(ProjectId projectId, CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public Task AddAsync(Project project, CancellationToken cancellationToken)
        {
            if (OnAddAsync is not null)
            {
                return OnAddAsync(project, cancellationToken);
            }

            AddedProject = project;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Project project, CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public Task RemoveAsync(Project project, CancellationToken cancellationToken)
            => throw new NotImplementedException();
    }

    [Fact]
    public async Task Handle_ShouldPersistProject_AndReturnItsId()
    {
        var repository = new FakeProjectRepository();
        var handler = new CreateProjectHandler(repository);

        var command = new CreateProjectCommand(
            Title: "Architect Portfolio Platform",
            ShortDescription: "short description",
            Description: "A portfolio platform demonstrating software architecture.",
            ProjectUrl: null,
            StartDate: new DateOnly(2026, 2, 1),
            EndDate: new DateOnly(2026, 8, 1));

        var result = await handler.Handle(command, CancellationToken.None);

        _output.WriteLine($"\n ################# \n Created project ID: {result.ProjectId} \n ################# \n");

        repository.AddedProject.Should()
            .NotBeNull();

        result.ProjectId.Should()
            .Be(repository.AddedProject!.Id.Value);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenTitleIsEmpty()
    {
        var repository = new FakeProjectRepository();
        var handler = new CreateProjectHandler(repository);

        var command = new CreateProjectCommand(
            Title: "",
            ShortDescription: "short description",
            Description: "A portfolio platform demonstrating software architecture.",
            ProjectUrl: null,
            StartDate: new DateOnly(2026, 2, 1),
            EndDate: new DateOnly(2026, 8, 1));

        var action = () => handler.Handle(command, CancellationToken.None);

        await action.Should()
            .ThrowAsync<ArgumentException>();

        repository.AddedProject.Should()
            .BeNull();
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenEndDatePrecedesStartDate()
    {
        var repository = new FakeProjectRepository();
        var handler = new CreateProjectHandler(repository);

        var command = new CreateProjectCommand(
            Title: "Architect Portfolio Platform",
            ShortDescription: "short description",
            Description: "A portfolio platform demonstrating software architecture.",
            ProjectUrl: null,
            StartDate: new DateOnly(2026, 8, 1),
            EndDate: new DateOnly(2026, 2, 1));

        var action = () => handler.Handle(command, CancellationToken.None);

        await action.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Project end date cannot precede start date.");

        repository.AddedProject.Should()
            .BeNull();
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenDescriptionIsEmpty()
    {
        var repository = new FakeProjectRepository();
        var handler = new CreateProjectHandler(repository);

        var command = new CreateProjectCommand(
            Title: "Architect Portfolio Platform",
            ShortDescription: "short description",
            Description: "aaaa",
            ProjectUrl: null,
            StartDate: new DateOnly(2026, 2, 1),
            EndDate: new DateOnly(2026, 8, 1));

        var action = () => handler.Handle(command, CancellationToken.None);

        await action.Should()
            .ThrowAsync<ArgumentException>();

        repository.AddedProject.Should()
            .BeNull();
    }

    [Fact]
    public async Task Handle_RepositoryFalure_ShouldThrowException()
    {
        var repository = new FakeProjectRepository();
        var handler = new CreateProjectHandler(repository);

        var command = new CreateProjectCommand(
            Title: "Architect Portfolio Platform",
            ShortDescription: "short description",
            Description: "A portfolio platform demonstrating software architecture.",
            ProjectUrl: null,
            StartDate: new DateOnly(2026, 2, 1),
            EndDate: new DateOnly(2026, 8, 1));

        repository.OnAddAsync = (project, cancellationToken) =>
            throw new InvalidOperationException("Simulated repository failure.");

        Func<Task> action = async () => await handler.Handle(command, CancellationToken.None);

        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Simulated repository failure.");
    }
}
