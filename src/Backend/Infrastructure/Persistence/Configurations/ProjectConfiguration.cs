using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.Entities;
using ArchitectPortfolioPlatform.Domain.Portfolio.Projects.ValueObjects;

namespace Infrastructure.persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => new ProjectId(value))
            .ValueGeneratedNever();

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(p => p.IsFeatured)
            .IsRequired();

        builder.OwnsOne(p => p.Metadata, metadata =>
        {
            metadata.Property(m => m.Title)
                .HasColumnName("Metadata_Title")
                .IsRequired()
                .HasMaxLength(200);

            metadata.Property(m => m.ShortDescription)
                .HasColumnName("Metadata_ShortDescription")
                .IsRequired()
                .HasMaxLength(500);

            metadata.Property(m => m.Description)
                .HasColumnName("Metadata_Description")
                .IsRequired();

            metadata.Property(m => m.ProjectUrl)
                .HasColumnName("Metadata_ProjectUrl")
                .HasConversion(
                    url => url == null ? null : url.ToString(),
                    value => value == null ? null : new Uri(value));
        });

        builder.OwnsOne(p => p.Duration, duration =>
        {
            duration.Property(d => d.StartDate)
                .HasColumnName("Duration_StartDate")
                .IsRequired();

            duration.Property(d => d.EndDate)
                .HasColumnName("Duration_EndDate");
        });

        builder.Metadata.FindNavigation(nameof(Project.Images))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(p => p.Images)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
