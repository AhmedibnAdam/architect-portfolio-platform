using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Entities;

namespace Infrastructure.persistence.Configurations;

public sealed class ProjectImageConfiguration : IEntityTypeConfiguration<ProjectImage>
{
    public void Configure(EntityTypeBuilder<ProjectImage> builder)
    {
        builder.ToTable("ProjectImages");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .ValueGeneratedNever();

        builder.Property(i => i.Url)
            .IsRequired();

        builder.Property(i => i.Caption)
            .HasMaxLength(500);

        builder.Property(i => i.DisplayOrder)
            .IsRequired();

        builder.Property(i => i.IsHero)
            .IsRequired();
    }
}
