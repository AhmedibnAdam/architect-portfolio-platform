// ### 4.2 Profile
// **Type:** Aggregate Root  

// **Conceptual model:**
// ```
// Profile
//  ├── Name
//  ├── ProfessionalTitle
//  ├── Summary
//  ├── Location
//  ├── Email
//  ├── Phone
//  ├── SocialLinks
//  └── CV
// ```


using ArchitectPortfolioPlatform.Domain.Common;
using ArchitectPortfolioPlatform.Domain.Portfolio.Profile.ValueObjects;
namespace ArchitectPortfolioPlatform.Domain.Portfolio.Profile.Entities;

public sealed class Profile
    : AggregateRoot<ProfileId>
{
    private readonly List<string> _socialLinks = [];

    public string Name { get; private set; }
    public string ProfessionalTitle { get; private set; }
    public string? Summary { get; private set; }
    public string? Location { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public Uri? CvUrl { get; private set; }

    public IReadOnlyCollection<string> SocialLinks =>
        _socialLinks.AsReadOnly();

    private Profile() : base(default)
    {
        Name = string.Empty;
        ProfessionalTitle = string.Empty;
    }

    private Profile(
        ProfileId id,
        string name,
        string professionalTitle,
        string? summary,
        string? location,
        string? email,
        string? phone,
        Uri? cvUrl) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(
                "Profile name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(professionalTitle))
        {
            throw new DomainException(
                "Profile professional title cannot be empty.");
        }

        Name = name;
        ProfessionalTitle = professionalTitle;
        Summary = summary;
        Location = location;
        Email = email;
        Phone = phone;
        CvUrl = cvUrl;
    }

    public static Profile Create(
        string name,
        string professionalTitle,
        string? summary = null,
        string? location = null,
        string? email = null,
        string? phone = null,
        Uri? cvUrl = null)
    {
        return new Profile(
            ProfileId.New(),
            name,
            professionalTitle,
            summary,
            location,
            email,
            phone,
            cvUrl);
    }

    public void UpdateBio(
        string professionalTitle,
        string? summary,
        string? location)
    {
        if (string.IsNullOrWhiteSpace(professionalTitle))
        {
            throw new DomainException(
                "Profile professional title cannot be empty.");
        }

        ProfessionalTitle = professionalTitle;
        Summary = summary;
        Location = location;
    }

    public void UpdateContactInfo(
        string? email,
        string? phone)
    {
        if (!string.IsNullOrWhiteSpace(email) && !email.Contains('@'))
        {
            throw new DomainException(
                "Profile email is not a valid email address.");
        }

        Email = email;
        Phone = phone;
    }

    public void UpdateCv(Uri? cvUrl)
    {
        CvUrl = cvUrl;
    }

    public void AddSocialLink(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new DomainException(
                "Social link cannot be empty.");
        }

        if (!_socialLinks.Contains(url))
        {
            _socialLinks.Add(url);
        }
    }
}
