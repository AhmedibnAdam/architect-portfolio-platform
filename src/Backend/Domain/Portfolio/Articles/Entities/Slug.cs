

namespace ArchitectPortfolioPlatform.Domain.Portfolio.Articles;

public sealed class Slug
{
    public string Value { get; }

    private Slug(string value)
    {
        Value = value;
    }

    public static Slug Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "Slug cannot be empty.",
                nameof(value));

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Contains(' '))
            throw new ArgumentException(
                "Slug cannot contain spaces.",
                nameof(value));

        return new Slug(normalized);
    }

    public override string ToString() => Value;
}