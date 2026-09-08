using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArchitectPortfolioPlatform.Domain.Portfolio.Experience.Entities;
using ArchitectPortfolioPlatform.Domain.Portfolio.Experience.ValueObjects;

namespace Infrastructure.persistence.Configurations;

public sealed class ExperienceConfiguration : IEntityTypeConfiguration<Experience>
{
    public void Configure(EntityTypeBuilder<Experience> builder)
    {
        builder.ToTable("Experiences");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                id => id.Value,
                value => new ExperienceId(value))
            .ValueGeneratedNever();

        builder.Property(e => e.Company)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Position)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.StartDate)
            .IsRequired();

        builder.Property(e => e.EndDate);

        builder.Property(e => e.IsCurrent)
            .IsRequired();

        builder.PrimitiveCollection(e => e.Responsibilities)
            .HasField("_responsibilities")
            .ElementType(b => b.HasMaxLength(1000));

        builder.PrimitiveCollection(e => e.Achievements)
            .HasField("_achievements")
            .ElementType(b => b.HasMaxLength(1000));
    }
}
