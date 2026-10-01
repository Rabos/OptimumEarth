using System.Net;
using Microsoft.AspNetCore.Http;
using OptimumEarth.Web.Data;

namespace OptimumEarth.Web.Areas.Admin.Content;

public enum FieldKind
{
    Text,
    Area,
    Select,
    Image,
    Bool,
    Lines,
    Date,
    Number,
    Markdown,

    /// <summary>The "Appears on" checkboxes. Not a property: handled by the descriptor's hooks.</summary>
    Placements,
}

/// <summary>One editable field. Key is the entity property name (or a hook-handled key for Placements).</summary>
public sealed class FieldDef
{
    public required string Key { get; init; }
    public required string Label { get; init; }
    public FieldKind Kind { get; init; } = FieldKind.Text;
    public bool Required { get; init; }

    /// <summary>Maximum characters (Text, Area, Markdown).</summary>
    public int Max { get; init; }
    public int Min { get; init; }
    public int MaxValue { get; init; }
    public bool Half { get; init; }
    public int Rows { get; init; }
    public string? Hint { get; init; }

    /// <summary>Heading shown above this field, starting a new group.</summary>
    public string? Group { get; init; }

    /// <summary>Fixed choices for a Select.</summary>
    public IReadOnlyList<(string Value, string Label)>? Options { get; init; }

    /// <summary>Name of a dynamic option list (see <see cref="OptionSources"/>).</summary>
    public string? OptionsSource { get; init; }

    public string? Pattern { get; init; }
    public string? PatternMessage { get; init; }
}

public sealed class FilterDef<T>
{
    public required string Key { get; init; }
    public required string Label { get; init; }
    public IReadOnlyList<(string Value, string Label)>? Options { get; init; }
    public string? OptionsSource { get; init; }
    public required Func<IQueryable<T>, string, IQueryable<T>> Apply { get; init; }
}

/// <summary>A table column. Html returns already-encoded markup (use <see cref="Cell.E"/>).</summary>
public sealed class ColumnDef<T>
{
    public required string Header { get; init; }
    public required Func<T, object?, string> Html { get; init; }
}

public static class Cell
{
    public static string E(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);

    public static string Thumb(string? path, string title, string? sub = null) =>
        $"<div class=\"cellrow\"><div class=\"thumb\" style=\"background-image:url('{E(path)}')\"></div><div><b>{E(title)}</b>{(sub is null ? "" : $"<div class=\"mono muted\">{E(sub)}</div>")}</div></div>";

    public static string Title(string title, string? sub = null) =>
        $"<b>{E(title)}</b>{(sub is null ? "" : $"<div class=\"mono muted\">{E(sub)}</div>")}";

    public static string Tag(string text, string cls = "t-grey") => $"<span class=\"tag {cls}\">{E(text)}</span>";
}

public sealed record StatusFilter(string Key, string Label);

public sealed record PlacementOption(string Value, string Label, bool Checked, bool Disabled, string? Note = null);

/// <summary>Reads posted fields, which are named f_{Key}.</summary>
public sealed class FormView
{
    private readonly IFormCollection _form;

    public FormView(IFormCollection form)
    {
        _form = form;
    }

    public static string NameOf(string key) => "f_" + key;

    public bool Has(string key) => _form.ContainsKey(NameOf(key));

    public string Get(string key) => _form[NameOf(key)].ToString();

    public IReadOnlyList<string> Many(string key) => _form[NameOf(key)].ToList()!;

    public bool IsChecked(string key) => _form[NameOf(key)].Any(v => v == "true" || v == "on");
}

public sealed class ListQuery
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 25;
    public string Search { get; set; } = string.Empty;
    public string Status { get; set; } = "all";
    public Dictionary<string, string> Filters { get; } = new();

    public static readonly int[] Sizes = { 10, 25, 50, 100 };

    public bool IsFiltered => !string.IsNullOrEmpty(Search) || Status != "all" || Filters.Values.Any(v => !string.IsNullOrEmpty(v) && v != "all");
}

public sealed record RowView(int Id, string StatusKey, string StatusLabel, IReadOnlyList<string> Cells);

public sealed record ListResult(IReadOnlyList<RowView> Rows, int Total, int Page, int Size);

public sealed class EditData
{
    public int? Id { get; init; }
    public string StatusKey { get; init; } = "new";
    public string StatusLabel { get; init; } = "New";
    public string Title { get; init; } = string.Empty;
    public Dictionary<string, string> Values { get; init; } = new();
    public List<PlacementOption> Placements { get; set; } = new();
}

public sealed class SaveOutcome
{
    public Dictionary<string, string> Errors { get; } = new();
    public int? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsNew { get; set; }
    public bool Succeeded => Errors.Count == 0;
}

public enum SaveMode
{
    /// <summary>Keep the current status (settings and other single records).</summary>
    Keep,
    Draft,
    Publish,
}
