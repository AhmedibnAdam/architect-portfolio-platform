

using ArchitectPortfolioPlatform.Domain.Common;
using ArchitectPortfolioPlatform.Domain.Portfolio.Skills.ValueObjects;
using System;
namespace ArchitectPortfolioPlatform.Domain.Portfolio.Skills.Entities;

public sealed class Skill
    : AggregateRoot<SkillId>
{
    public string Name { get; private set; }
    public string Category { get; private set; }

    public string Proficiency { get; private set; }

    public string? Description { get; private set; }

    private Skill() : base(default)
    {
        Name = string.Empty;
        Category = string.Empty;
        Proficiency = string.Empty;
    }

    private Skill(
            SkillId id,
            string name,
            string category,
            string proficiency,
            string? description) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(
                "Skill name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            throw new DomainException(
                "Skill category cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(proficiency))
        {
            throw new DomainException(
                "Skill proficiency cannot be empty.");
        }

        Name = name;
        Category = category;
        Proficiency = proficiency;
        Description = description;
    }


    public static Skill Create(
            string name,
            string category,
            string proficiency,
            string? description = null)
    {
        return new Skill(
            SkillId.New(),
            name,
            category,
            proficiency,
            description);
    }

    public void Update(
            string name,
            string category,
            string proficiency,
            string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(
                "Skill name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            throw new DomainException(
                "Skill category cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(proficiency))
        {
            throw new DomainException(
                "Skill proficiency cannot be empty.");
        }

        Name = name;
        Category = category;
        Proficiency = proficiency;
        Description = description;
    }

}