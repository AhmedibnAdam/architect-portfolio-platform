using ArchitectPortfolioPlatform.Domain.Portfolio.Experience.Entities;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Entities;
using ArchitectPortfolioPlatform.Domain.Portfolio.Skills.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Backend.IntegrationTests.Persistence;

[Collection(nameof(PostgresCollection))]
public sealed class PortfolioDbContextTests
{
    private readonly PostgresDatabaseFixture _fixture;

    public PortfolioDbContextTests(PostgresDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Context_can_connect_to_PostgreSQL()
    {
        await using var context = _fixture.CreateContext();

        var canConnect = await context.Database.CanConnectAsync();

        canConnect.Should().BeTrue();
    }

    [Fact]
    public void Model_discovers_configurations_for_all_aggregates()
    {
        using var context = _fixture.CreateContext();

        var entityTypes = context.Model.GetEntityTypes().Select(e => e.ClrType).ToList();

        entityTypes.Should().Contain(typeof(Project));
        entityTypes.Should().Contain(typeof(ProjectImage));
        entityTypes.Should().Contain(typeof(Skill));
        entityTypes.Should().Contain(typeof(Experience));
    }

    [Fact]
    public void Project_owned_types_are_mapped_as_owned_entities()
    {
        using var context = _fixture.CreateContext();

        var projectType = context.Model.FindEntityType(typeof(Project));

        var ownedNavigations = projectType!.GetNavigations()
            .Where(n => n.ForeignKey.IsOwnership)
            .Select(n => n.Name)
            .ToList();

        ownedNavigations.Should().Contain("Metadata");
        ownedNavigations.Should().Contain("Duration");
    }

    [Fact]
    public async Task Skill_can_be_inserted_and_read_back()
    {
        var skill = Skill.Create("C#", "Backend", "Expert", "Primary language");

        await using (var context = _fixture.CreateContext())
        {
            context.Skills.Add(skill);
            await context.SaveChangesAsync();
        }

        await using (var context = _fixture.CreateContext())
        {
            var loaded = await context.Skills.SingleAsync(s => s.Id == skill.Id);

            loaded.Name.Should().Be("C#");
            loaded.Category.Should().Be("Backend");
            loaded.Proficiency.Should().Be("Expert");
            loaded.Description.Should().Be("Primary language");
        }
    }

    [Fact]
    public async Task Experience_can_be_inserted_and_read_back_with_collections()
    {
        var experience = Experience.Create(
            "Acme Corp",
            "Software Architect",
            "Led backend platform",
            new DateOnly(2020, 1, 1),
            new DateOnly(2023, 6, 30));

        experience.AddResponsibility("Design system architecture");
        experience.AddAchievement("Reduced latency by 40%");

        await using (var context = _fixture.CreateContext())
        {
            context.Experiences.Add(experience);
            await context.SaveChangesAsync();
        }

        await using (var context = _fixture.CreateContext())
        {
            var loaded = await context.Experiences.SingleAsync(e => e.Id == experience.Id);

            loaded.Company.Should().Be("Acme Corp");
            loaded.Position.Should().Be("Software Architect");
            loaded.IsCurrent.Should().BeFalse();
            loaded.Responsibilities.Should().Contain("Design system architecture");
            loaded.Achievements.Should().Contain("Reduced latency by 40%");
        }
    }

    [Fact]
    public async Task Experience_with_no_end_date_is_persisted_as_current()
    {
        var experience = Experience.Create(
            "Current Co",
            "Principal Engineer",
            null,
            new DateOnly(2024, 1, 1));

        await using (var context = _fixture.CreateContext())
        {
            context.Experiences.Add(experience);
            await context.SaveChangesAsync();
        }

        await using (var context = _fixture.CreateContext())
        {
            var loaded = await context.Experiences.SingleAsync(e => e.Id == experience.Id);

            loaded.EndDate.Should().BeNull();
            loaded.IsCurrent.Should().BeTrue();
        }
    }
}
