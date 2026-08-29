namespace OptimumEarth.Models;

public class CountryInfo
{
    public string Key { get; set; } = "";
    public string Eyebrow { get; set; } = "";
    public string Headline { get; set; } = "";
    public string Sub { get; set; } = "";
    public string Overview { get; set; } = "";
    public string Overview2 { get; set; } = "";
    public List<Fact> Facts { get; set; } = new();
    public string ServicesTitle { get; set; } = "";
    public List<ServiceItem> Services { get; set; } = new();
    public string ProjectsTitle { get; set; } = "";
    public List<CountryProject> Projects { get; set; } = new();
    public string ProjectsNote { get; set; } = "";
    public string CtaTitle { get; set; } = "";
    public string Contact { get; set; } = "";
}

public class Fact
{
    public string K { get; set; } = "";
    public string V { get; set; } = "";
}

public class ServiceItem
{
    public string T { get; set; } = "";
    public string D { get; set; } = "";
}

public class CountryProject
{
    public string Tag { get; set; } = "";
    public string Title { get; set; } = "";
    public string Meta { get; set; } = "";
}
