namespace ArchitectPortfolioPlatform.Domain.Portfolio.Projects.ValueObjects;

public sealed record ProjectMetadata(
    string Client,
    string Location,
    string ArchitectureType);