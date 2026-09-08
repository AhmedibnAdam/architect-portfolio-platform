using ArchitectPortfolioPlatform.Domain.Common;
using ArchitectPortfolioPlatform.Domain.Portfolio.Skills.Entities;

using FluentAssertions;

namespace Backend.Tests.Portfolio.Skills;

public class SkillTests
{
    private static Skill CreateValidSkill()
    {
        return Skill.Create(
            name: "C#",
            category: "Programming Language",
            proficiency: "Advanced",
            description: "A programming language developed by Microsoft.");
    }

    [Fact]
    public void Create_ShouldFail_WhenNameIsEmpty()
    {
        var action = () => Skill.Create(
            name: "",
            category: "Programming Language",
            proficiency: "Advanced",
            description: "A programming language developed by Microsoft.");

        action.Should()
            .Throw<DomainException>()
            .WithMessage("Skill name cannot be empty.");
    }

    [Fact]
    public void Create_ShouldFail_WhenCategoryIsEmpty()
    {
        var action = () => Skill.Create(
            name: "C#",
            category: "",
            proficiency: "Advanced",
            description: "A programming language developed by Microsoft.");

        action.Should()
            .Throw<DomainException>()
            .WithMessage("Skill category cannot be empty.");
    }

    [Fact]
    public void Create_ShouldFail_WhenProficiencyIsEmpty()
    {
        var action = () => Skill.Create(
            name: "C#",
            category: "Programming Language",
            proficiency: "",
            description: "A programming language developed by Microsoft.");

        action.Should()
            .Throw<DomainException>()
            .WithMessage("Skill proficiency cannot be empty.");
    }

    [Fact]
    public void Create_ShouldSetAllProperties_WhenAllParametersAreValid()
    {
        var skill = CreateValidSkill();

        skill.Name.Should()
            .Be("C#");

        skill.Category.Should()
            .Be("Programming Language");

        skill.Proficiency.Should()
            .Be("Advanced");

        skill.Description.Should()
            .Be("A programming language developed by Microsoft.");
    }

    [Fact]
    public void Create_ShouldSucceed_WhenDescriptionIsNotProvided()
    {
        var skill = Skill.Create(
            name: "C#",
            category: "Programming Language",
            proficiency: "Advanced");

        skill.Description.Should()
            .BeNull();
    }

    [Fact]
    public void Update_ShouldFail_WhenNameIsEmpty()
    {
        var skill = CreateValidSkill();

        var action = () => skill.Update(
            name: "",
            category: "Programming Language",
            proficiency: "Advanced",
            description: "A programming language developed by Microsoft.");

        action.Should()
            .Throw<DomainException>()
            .WithMessage("Skill name cannot be empty.");
    }

    [Fact]
    public void Update_ShouldFail_WhenCategoryIsEmpty()
    {
        var skill = CreateValidSkill();

        var action = () => skill.Update(
            name: "C#",
            category: "",
            proficiency: "Advanced",
            description: "A programming language developed by Microsoft.");

        action.Should()
            .Throw<DomainException>()
            .WithMessage("Skill category cannot be empty.");
    }

    [Fact]
    public void Update_ShouldFail_WhenProficiencyIsEmpty()
    {
        var skill = CreateValidSkill();

        var action = () => skill.Update(
            name: "C#",
            category: "Programming Language",
            proficiency: "",
            description: "A programming language developed by Microsoft.");

        action.Should()
            .Throw<DomainException>()
            .WithMessage("Skill proficiency cannot be empty.");
    }

    [Fact]
    public void Update_ShouldSetAllProperties_WhenAllParametersAreValid()
    {
        var skill = CreateValidSkill();

        skill.Update(
            name: "Python",
            category: "Programming Language",
            proficiency: "Intermediate",
            description: "A high-level programming language.");

        skill.Name.Should()
            .Be("Python");

        skill.Category.Should()
            .Be("Programming Language");

        skill.Proficiency.Should()
            .Be("Intermediate");

        skill.Description.Should()
            .Be("A high-level programming language.");
    }

    [Fact]
    public void Update_ShouldClearDescription_WhenDescriptionIsNotProvided()
    {
        var skill = CreateValidSkill();

        skill.Update(
            name: "C#",
            category: "Programming Language",
            proficiency: "Advanced");

        skill.Description.Should()
            .BeNull();
    }
}
