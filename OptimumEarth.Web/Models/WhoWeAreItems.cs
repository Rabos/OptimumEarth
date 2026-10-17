namespace OptimumEarth.Web.Models;

/// <summary>A point on the Who We Are timeline.</summary>
public record MilestoneItem(string Year, string Title, string Description);

/// <summary>A company or Foundation card in the Who We Are "group" section.</summary>
public record GroupCard(string Kind, string Name, string Description, IReadOnlyList<string> Tags, string LinkText, string LinkUrl, string ImagePath, string ImageAlt, bool IsFoundation);

/// <summary>A counter in the stats bands. CountsYears means the value is worked out from the founding date when shown.</summary>
public record StatItem(bool CountsYears, int Value, string Suffix, string Label)
{
    public int ValueOn(DateOnly founded, DateOnly today) =>
        CountsYears ? Math.Max(0, today.Year - founded.Year - (today < founded.AddYears(today.Year - founded.Year) ? 1 : 0)) : Value;
}

/// <summary>An office, as shown in the footer and on the Who We Are page.</summary>
public record OfficeInfo(string Name, string Country, IReadOnlyList<string> AddressLines, string Phone, string Email)
{
    public string PhoneHref => "tel:" + new string(Phone.Where(c => char.IsDigit(c) || c == '+').ToArray());
}

/// <summary>The free text on the Who We Are page.</summary>
public record WhoPageText(
    string Headline,
    string Highlight,
    string Lede,
    string HeroImagePath,
    string HeroImageAlt,
    string StoryHeadline,
    IReadOnlyList<string> StoryParagraphs,
    string StoryImagePath,
    string StoryInsetImagePath,
    DateOnly FoundedOn,
    IReadOnlyList<string> WorkCountries,
    string CtaHeadline,
    string CtaBody,
    string CtaImagePath);
