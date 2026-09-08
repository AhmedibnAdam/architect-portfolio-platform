using ArchitectPortfolioPlatform.Domain.Common;
using ArchitectPortfolioPlatform.Domain.Portfolio.Experience.Entities;

using FluentAssertions;

namespace Backend.Tests.Portfolio.Experience;

public class ExperienceTests
{
    private static global::ArchitectPortfolioPlatform.Domain.Portfolio.Experience.Entities.Experience CreateValidExperience()
    {
        return global::ArchitectPortfolioPlatform.Domain.Portfolio.Experience.Entities.Experience.Create(
            company: "Acme Corp",
            position: "Software Architect",
            description: "Led the platform architecture team.",
            startDate: new DateOnly(2024, 1, 1));
    }

    [Fact]
    public void Create_ShouldFail_WhenCompanyIsEmpty()
    {
        var action = () => global::ArchitectPortfolioPlatform.Domain.Portfolio.Experience.Entities.Experience.Create(
            "",
            "Software Architect",
            "Description",
            new DateOnly(2024, 1, 1));

        action.Should()
            .Throw<DomainException>()
            .WithMessage("Experience company cannot be empty.");
    }

    [Fact]
    public void Create_ShouldFail_WhenPositionIsEmpty()
    {
        var action = () => global::ArchitectPortfolioPlatform.Domain.Portfolio.Experience.Entities.Experience.Create(
            "Acme Corp",
            "",
            "Description",
            new DateOnly(2024, 1, 1));

        action.Should()
            .Throw<DomainException>()
            .WithMessage("Experience position cannot be empty.");
    }

    [Fact]
    public void Create_ShouldFail_WhenEndDatePrecedesStartDate()
    {
        var action = () => global::ArchitectPortfolioPlatform.Domain.Portfolio.Experience.Entities.Experience.Create(
            "Acme Corp",
            "Software Architect",
            "Description",
            new DateOnly(2024, 6, 1),
            new DateOnly(2024, 1, 1));

        action.Should()
            .Throw<DomainException>()
            .WithMessage("Experience end date cannot precede start date.");
    }

    [Fact]
    public void Create_ShouldMarkAsCurrent_WhenNoEndDateProvided()
    {
        var experience = CreateValidExperience();

        experience.IsCurrent.Should()
            .BeTrue();

        experience.EndDate.Should()
            .BeNull();

        experience.Responsibilities.Should()
            .BeEmpty();

        experience.Achievements.Should()
            .BeEmpty();
    }

    [Fact]
    public void Create_ShouldNotBeCurrent_WhenEndDateProvided()
    {
        var experience = global::ArchitectPortfolioPlatform.Domain.Portfolio.Experience.Entities.Experience.Create(
            "Acme Corp",
            "Software Architect",
            "Description",
            new DateOnly(2024, 1, 1),
            new DateOnly(2024, 12, 31));

        experience.IsCurrent.Should()
            .BeFalse();
    }

    [Fact]
    public void AddResponsibility_ShouldAddResponsibility()
    {
        var experience = CreateValidExperience();

        experience.AddResponsibility("Design system architecture");

        experience.Responsibilities.Should()
            .ContainSingle()
            .Which.Should()
            .Be("Design system architecture");
    }

    [Fact]
    public void AddResponsibility_ShouldNotAddDuplicate()
    {
        var experience = CreateValidExperience();

        experience.AddResponsibility("Design system architecture");
        experience.AddResponsibility("Design system architecture");

        experience.Responsibilities.Should()
            .ContainSingle();
    }

    [Fact]
    public void AddResponsibility_ShouldFail_WhenEmpty()
    {
        var experience = CreateValidExperience();

        var action = () => experience.AddResponsibility("");

        action.Should()
            .Throw<DomainException>()
            .WithMessage("Responsibility cannot be empty.");
    }

    [Fact]
    public void AddAchievement_ShouldAddAchievement()
    {
        var experience = CreateValidExperience();

        experience.AddAchievement("Reduced deployment time by 40%");

        experience.Achievements.Should()
            .ContainSingle()
            .Which.Should()
            .Be("Reduced deployment time by 40%");
    }

    [Fact]
    public void AddAchievement_ShouldNotAddDuplicate()
    {
        var experience = CreateValidExperience();

        experience.AddAchievement("Reduced deployment time by 40%");
        experience.AddAchievement("Reduced deployment time by 40%");

        experience.Achievements.Should()
            .ContainSingle();
    }

    [Fact]
    public void AddAchievement_ShouldFail_WhenEmpty()
    {
        var experience = CreateValidExperience();

        var action = () => experience.AddAchievement("");

        action.Should()
            .Throw<DomainException>()
            .WithMessage("Achievement cannot be empty.");
    }

    [Fact]
    public void EndRole_ShouldSetEndDateAndClearIsCurrent()
    {
        var experience = CreateValidExperience();

        experience.EndRole(new DateOnly(2024, 12, 31));

        experience.IsCurrent.Should()
            .BeFalse();

        experience.EndDate.Should()
            .Be(new DateOnly(2024, 12, 31));
    }

    [Fact]
    public void EndRole_ShouldFail_WhenEndDatePrecedesStartDate()
    {
        var experience = CreateValidExperience();

        var action = () => experience.EndRole(new DateOnly(2023, 1, 1));

        action.Should()
            .Throw<DomainException>()
            .WithMessage("Experience end date cannot precede start date.");
    }
}
