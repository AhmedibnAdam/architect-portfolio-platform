using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArchitectPortfolioPlatform.Domain.Portfolio.Skills.Entities;
using ArchitectPortfolioPlatform.Domain.Portfolio.Skills.ValueObjects;

namespace Infrastructure.persistence.Configurations;

public sealed class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("Skills");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => new SkillId(value))
            .ValueGeneratedNever();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Category)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Proficiency)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Description)
            .HasMaxLength(1000);
    }
}
