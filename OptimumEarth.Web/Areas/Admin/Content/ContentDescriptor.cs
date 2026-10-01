using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using OptimumEarth.Web.Data;
using OptimumEarth.Web.Services;

namespace OptimumEarth.Web.Areas.Admin.Content;

public sealed record DeleteResult(string? Error, string Title);

public sealed record FilterInfo(string Key, string Label, IReadOnlyList<(string Value, string Label)>? Options, string? OptionsSource);

/// <summary>What the generic list and edit pages need to know about a content type.</summary>
public interface IContentDescriptor
{
    string Slug { get; }
    string Label { get; }
    string Singular { get; }
    string Area { get; }
    string Description { get; }
    bool Single { get; }
    bool HasStatus { get; }
    bool Orderable { get; }
    bool CanAdd { get; }
    bool CanDelete { get; }
    int? MaxItems { get; }
    IReadOnlyList<FieldDef> Fields { get; }
    IReadOnlyList<string> ColumnHeaders { get; }
    IReadOnlyList<StatusFilter> StatusFilters { get; }
    IReadOnlyList<FilterInfo> Filters { get; }

    Task<ListResult> ListAsync(AppDbContext db, ListQuery query);
    Task<int> CountAsync(AppDbContext db);
    Task<EditData?> LoadAsync(AppDbContext db, AccessSnapshot access, int? id);
    Task<EditData> FromFormAsync(AppDbContext db, AccessSnapshot access, int? id, FormView form);
    Task<SaveOutcome> SaveAsync(AppDbContext db, AccessSnapshot access, int? id, FormView form, SaveMode mode);
    Task<DeleteResult> DeleteAsync(AppDbContext db, int id);
    Task MoveAsync(AppDbContext db, int id, int delta, ListQuery scope);
    Task<List<(string Value, string Label)>> OptionsAsync(AppDbContext db, FieldDef field, string? current);
}

/// <summary>
/// Describes one kind of content (slides, projects, posts...) once, and
/// provides list, edit, save, delete and reorder for it from that description.
/// Entity properties are read and written by name, so a new content type is a
/// short declaration rather than a new pair of pages.
/// </summary>
public sealed class ContentDescriptor<T> : IContentDescriptor where T : class, new()
{
    private static readonly Regex SlugCleaner = new("[^a-z0-9]+", RegexOptions.Compiled);

    private readonly Dictionary<string, PropertyInfo> _props = typeof(T).GetProperties().ToDictionary(p => p.Name);

    public required string Slug { get; init; }
    public required string Label { get; init; }
    public required string Singular { get; init; }
    public required string Area { get; init; }
    public string Description { get; init; } = string.Empty;
    public required Func<AppDbContext, DbSet<T>> Set { get; init; }
    public required IReadOnlyList<FieldDef> Fields { get; init; }
    public required IReadOnlyList<ColumnDef<T>> Columns { get; init; }
    public required Func<T, string> TitleOf { get; init; }

    public bool Single { get; init; }
    public bool Orderable { get; init; } = true;
    public bool CanAdd { get; init; } = true;
    public bool CanDelete { get; init; } = true;
    public int? MaxItems { get; init; }

    /// <summary>Applies the search box. Null means no search.</summary>
    public Func<IQueryable<T>, string, IQueryable<T>>? Search { get; init; }

    public IReadOnlyList<FilterDef<T>> FilterDefs { get; init; } = Array.Empty<FilterDef<T>>();

    /// <summary>Replaces the default Published/Draft filter (the blog adds Scheduled and uses the publish date).</summary>
    public IReadOnlyList<StatusFilter>? CustomStatuses { get; init; }

    public Func<IQueryable<T>, string, IQueryable<T>>? StatusQuery { get; init; }
    public Func<T, string>? StatusKeyOf { get; init; }

    /// <summary>Overrides the default SortOrder ordering of the list.</summary>
    public Func<IQueryable<T>, IQueryable<T>>? DefaultOrder { get; init; }

    /// <summary>Property names that must be unique among all rows (e.g. Slug).</summary>
    public IReadOnlyList<string> Unique { get; init; } = Array.Empty<string>();

    /// <summary>Fill <c>Target</c> from <c>Source</c> when left empty, as a URL slug.</summary>
    public (string Source, string Target)? AutoSlug { get; init; }

    /// <summary>Loads extra data for the page of rows (one query), handed to each column.</summary>
    public Func<AppDbContext, IReadOnlyList<T>, Task<object?>>? PrepareColumns { get; init; }

    public ContentHooks<T>? Hooks { get; init; }

    public bool HasStatus => _props.ContainsKey("Status");

    public IReadOnlyList<string> ColumnHeaders => Columns.Select(c => c.Header).ToList();

    public IReadOnlyList<StatusFilter> StatusFilters => CustomStatuses ?? new[]
    {
        new StatusFilter("published", "Published"),
        new StatusFilter("draft", "Drafts"),
    };

    public IReadOnlyList<FilterInfo> Filters => FilterDefs.Select(f => new FilterInfo(f.Key, f.Label, f.Options, f.OptionsSource)).ToList();

    private int IdOf(T entity) => (int)_props["Id"].GetValue(entity)!;

    private ContentStatus StatusOf(T entity) => HasStatus ? (ContentStatus)_props["Status"].GetValue(entity)! : ContentStatus.Published;

    private string StatusKey(T entity) => StatusKeyOf?.Invoke(entity) ?? (StatusOf(entity) == ContentStatus.Draft ? "draft" : "published");

    private static string StatusText(string key) => key switch
    {
        "draft" => "Draft",
        "scheduled" => "Scheduled",
        "published" => "Published",
        _ => key,
    };

    // ---------- Listing ----------

    public async Task<int> CountAsync(AppDbContext db) => await Set(db).CountAsync();

    public async Task<ListResult> ListAsync(AppDbContext db, ListQuery q)
    {
        var query = Filtered(Set(db).AsNoTracking(), q);

        var total = await query.CountAsync();
        var ordered = DefaultOrder is not null
            ? DefaultOrder(query)
            : query.OrderBy(x => EF.Property<int>(x, "SortOrder")).ThenBy(x => EF.Property<int>(x, "Id"));

        var size = ListQuery.Sizes.Contains(q.Size) ? q.Size : 25;
        var pages = Math.Max(1, (int)Math.Ceiling(total / (double)size));
        var page = Math.Min(Math.Max(1, q.Page), pages);
        var items = await ordered.Skip((page - 1) * size).Take(size).ToListAsync();
        var context = PrepareColumns is null ? null : await PrepareColumns(db, items);

        var rows = items.Select(e =>
        {
            var key = StatusKey(e);
            return new RowView(IdOf(e), key, StatusText(key), Columns.Select(c => c.Html(e, context)).ToList());
        }).ToList();

        return new ListResult(rows, total, page, size);
    }

    // ---------- Loading for the form ----------

    public async Task<EditData?> LoadAsync(AppDbContext db, AccessSnapshot access, int? id)
    {
        T? entity;
        if (Single)
        {
            entity = await Set(db).AsNoTracking().FirstOrDefaultAsync();
            id = entity is null ? null : IdOf(entity);
        }
        else if (id is null)
        {
            entity = null;
        }
        else
        {
            entity = await Set(db).AsNoTracking().FirstOrDefaultAsync(x => EF.Property<int>(x, "Id") == id);
            if (entity is null)
            {
                return null;
            }
        }

        var source = entity ?? new T();
        var values = new Dictionary<string, string>();
        foreach (var f in Fields.Where(f => f.Kind != FieldKind.Placements))
        {
            values[f.Key] = ToText(_props[f.Key].GetValue(source));
        }

        if (entity is null && Hooks is not null)
        {
            foreach (var (k, v) in Hooks.Defaults(access))
            {
                values[k] = v;
            }
        }

        var key = entity is null ? "new" : StatusKey(entity);
        return new EditData
        {
            Id = id,
            StatusKey = key,
            StatusLabel = entity is null ? "New" : StatusText(key),
            Title = entity is null ? $"New {Singular.ToLowerInvariant()}" : TitleOf(entity),
            Values = values,
            Placements = Hooks is null ? new() : await Hooks.PlacementOptionsAsync(db, access, entity),
        };
    }

    /// <summary>The form as posted, so a failed save shows what the person typed.</summary>
    public async Task<EditData> FromFormAsync(AppDbContext db, AccessSnapshot access, int? id, FormView form)
    {
        var existing = id is null ? null : await Set(db).AsNoTracking().FirstOrDefaultAsync(x => EF.Property<int>(x, "Id") == id);
        var values = new Dictionary<string, string>();
        foreach (var f in Fields.Where(f => f.Kind != FieldKind.Placements))
        {
            values[f.Key] = f.Kind == FieldKind.Bool ? (form.IsChecked(f.Key) ? "true" : "false") : form.Get(f.Key);
        }

        var key = existing is null ? "new" : StatusKey(existing);
        var placements = Hooks is null ? new() : await Hooks.PlacementOptionsAsync(db, access, existing);
        var ticked = form.Many("shownOn").ToHashSet();
        if (ticked.Count > 0 || form.Has("shownOn") || placements.Count > 0)
        {
            placements = placements.Select(p => p with { Checked = p.Disabled ? p.Checked : ticked.Contains(p.Value) }).ToList();
        }

        return new EditData
        {
            Id = id,
            StatusKey = key,
            StatusLabel = existing is null ? "New" : StatusText(key),
            Title = existing is null ? $"New {Singular.ToLowerInvariant()}" : TitleOf(existing),
            Values = values,
            Placements = placements,
        };
    }

    // ---------- Saving ----------

    public async Task<SaveOutcome> SaveAsync(AppDbContext db, AccessSnapshot access, int? id, FormView form, SaveMode mode)
    {
        var outcome = new SaveOutcome();
        T? entity = null;
        if (Single)
        {
            entity = await Set(db).FirstOrDefaultAsync();
        }
        else if (id is not null)
        {
            entity = await Set(db).FirstOrDefaultAsync(x => EF.Property<int>(x, "Id") == id);
            if (entity is null)
            {
                outcome.Errors[string.Empty] = "That item no longer exists.";
                return outcome;
            }
        }

        var isNew = entity is null;
        var converted = new Dictionary<string, object?>();

        foreach (var f in Fields.Where(f => f.Kind != FieldKind.Placements))
        {
            var prop = _props[f.Key];
            var error = Convert(f, prop, form, out var value);
            if (error is not null)
            {
                outcome.Errors[f.Key] = error;
            }
            else
            {
                converted[f.Key] = value;
            }
        }

        if (AutoSlug is { } slug && outcome.Errors.Count == 0 && string.IsNullOrWhiteSpace(converted[slug.Target] as string))
        {
            converted[slug.Target] = Slugify(converted[slug.Source] as string ?? string.Empty);
        }

        if (outcome.Errors.Count == 0)
        {
            foreach (var key in Unique)
            {
                var value = converted[key] as string;
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                var currentId = entity is null ? 0 : IdOf(entity);
                var taken = await Set(db).AnyAsync(x => EF.Property<string>(x, key) == value && EF.Property<int>(x, "Id") != currentId);
                if (taken)
                {
                    outcome.Errors[key] = "Another item already uses this. Choose a different one.";
                }
            }
        }

        if (isNew && MaxItems is { } max && await Set(db).CountAsync() >= max)
        {
            outcome.Errors[string.Empty] = $"The page layout holds at most {max} {Label.ToLowerInvariant()}.";
        }

        if (Hooks is not null && outcome.Errors.Count == 0)
        {
            await Hooks.ValidateAsync(db, access, entity, form, outcome.Errors);
        }

        if (!outcome.Succeeded)
        {
            return outcome;
        }

        entity ??= new T();
        foreach (var (key, value) in converted)
        {
            _props[key].SetValue(entity, value);
        }

        if (isNew && _props.TryGetValue("SortOrder", out var sortProp))
        {
            var highest = await Set(db).MaxAsync(x => (int?)EF.Property<int>(x, "SortOrder")) ?? 0;
            sortProp.SetValue(entity, highest + 1);
        }

        if (HasStatus && mode != SaveMode.Keep)
        {
            _props["Status"].SetValue(entity, mode == SaveMode.Draft ? ContentStatus.Draft : ContentStatus.Published);
        }

        if (_props.TryGetValue("UpdatedUtc", out var updated))
        {
            updated.SetValue(entity, DateTime.UtcNow);
        }

        if (isNew)
        {
            Set(db).Add(entity);
        }

        await db.SaveChangesAsync();

        if (Hooks is not null)
        {
            await Hooks.AfterSaveAsync(db, access, entity, form);
            await db.SaveChangesAsync();
        }

        outcome.Id = IdOf(entity);
        outcome.IsNew = isNew;
        outcome.Title = TitleOf(entity);
        return outcome;
    }

    private string? Convert(FieldDef f, PropertyInfo prop, FormView form, out object? value)
    {
        value = null;
        var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        if (f.Kind == FieldKind.Bool)
        {
            value = form.IsChecked(f.Key);
            return null;
        }

        if (f.Kind == FieldKind.Lines)
        {
            var lines = form.Get(f.Key).Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
            if (f.Required && lines.Count == 0)
            {
                return $"{f.Label} needs at least one line.";
            }

            value = lines;
            return null;
        }

        var raw = form.Get(f.Key).Trim();
        if (raw.Length == 0)
        {
            if (f.Required)
            {
                return $"{f.Label} is required.";
            }

            value = type == typeof(string) ? string.Empty : prop.PropertyType.IsValueType && Nullable.GetUnderlyingType(prop.PropertyType) is null ? Activator.CreateInstance(prop.PropertyType) : null;
            return null;
        }

        if (f.Max > 0 && raw.Length > f.Max)
        {
            return $"Keep {f.Label.ToLowerInvariant()} under {f.Max} characters.";
        }

        if (f.Pattern is not null && !Regex.IsMatch(raw, f.Pattern))
        {
            return f.PatternMessage ?? $"{f.Label} is not in the right format.";
        }

        if (f.Kind == FieldKind.Select && f.Options is not null && f.Options.All(o => o.Value != raw))
        {
            return $"Choose one of the listed options for {f.Label.ToLowerInvariant()}.";
        }

        if (type == typeof(string))
        {
            // Text areas keep their line breaks; everything is stored exactly as typed apart from trimming.
            value = raw.Replace("\r\n", "\n");
            return null;
        }

        if (type == typeof(int))
        {
            if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n))
            {
                return $"{f.Label} must be a whole number.";
            }

            if (n < f.Min || (f.MaxValue > 0 && n > f.MaxValue))
            {
                return f.MaxValue > 0 ? $"{f.Label} must be between {f.Min} and {f.MaxValue}." : $"{f.Label} must be at least {f.Min}.";
            }

            value = n;
            return null;
        }

        if (type == typeof(DateOnly))
        {
            if (!DateOnly.TryParseExact(raw, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
            {
                return $"{f.Label} must be a date.";
            }

            value = d;
            return null;
        }

        return $"{f.Label} has an unsupported type.";
    }

    // ---------- Delete and reorder ----------

    public async Task<DeleteResult> DeleteAsync(AppDbContext db, int id)
    {
        var entity = await Set(db).FirstOrDefaultAsync(x => EF.Property<int>(x, "Id") == id);
        if (entity is null || !CanDelete)
        {
            return new DeleteResult("That item no longer exists.", string.Empty);
        }

        var title = TitleOf(entity);
        if (Hooks is not null)
        {
            var blocked = await Hooks.BeforeDeleteAsync(db, entity);
            if (blocked is not null)
            {
                return new DeleteResult(blocked, title);
            }
        }

        Set(db).Remove(entity);
        await db.SaveChangesAsync();
        return new DeleteResult(null, title);
    }

    /// <summary>
    /// Swaps this item with its neighbour in the list as currently filtered, so
    /// reordering works inside a filter (for example one story's gallery) too.
    /// </summary>
    public async Task MoveAsync(AppDbContext db, int id, int delta, ListQuery scope)
    {
        if (!Orderable || !_props.TryGetValue("SortOrder", out var sort))
        {
            return;
        }

        var all = await Filtered(Set(db), scope).OrderBy(x => EF.Property<int>(x, "SortOrder")).ThenBy(x => EF.Property<int>(x, "Id")).ToListAsync();
        var index = all.FindIndex(x => IdOf(x) == id);
        var target = index + delta;
        if (index < 0 || target < 0 || target >= all.Count)
        {
            return;
        }

        var a = (int)sort.GetValue(all[index])!;
        var b = (int)sort.GetValue(all[target])!;
        if (a == b)
        {
            // Equal positions cannot be swapped; renumber the whole list first.
            var everything = await Set(db).OrderBy(x => EF.Property<int>(x, "SortOrder")).ThenBy(x => EF.Property<int>(x, "Id")).ToListAsync();
            for (var i = 0; i < everything.Count; i++)
            {
                sort.SetValue(everything[i], i + 1);
            }

            await db.SaveChangesAsync();
            all = await Filtered(Set(db), scope).OrderBy(x => EF.Property<int>(x, "SortOrder")).ThenBy(x => EF.Property<int>(x, "Id")).ToListAsync();
            index = all.FindIndex(x => IdOf(x) == id);
            target = index + delta;
            a = (int)sort.GetValue(all[index])!;
            b = (int)sort.GetValue(all[target])!;
        }

        sort.SetValue(all[index], b);
        sort.SetValue(all[target], a);
        await db.SaveChangesAsync();
    }

    private IQueryable<T> Filtered(IQueryable<T> query, ListQuery q)
    {
        if (HasStatus && q.Status != "all")
        {
            query = StatusQuery is not null
                ? StatusQuery(query, q.Status)
                : q.Status == "draft"
                    ? query.Where(x => EF.Property<ContentStatus>(x, "Status") == ContentStatus.Draft)
                    : query.Where(x => EF.Property<ContentStatus>(x, "Status") == ContentStatus.Published);
        }

        if (!string.IsNullOrWhiteSpace(q.Search) && Search is not null)
        {
            query = Search(query, q.Search.Trim());
        }

        foreach (var filter in FilterDefs)
        {
            if (q.Filters.TryGetValue(filter.Key, out var value) && !string.IsNullOrEmpty(value) && value != "all")
            {
                query = filter.Apply(query, value);
            }
        }

        return query;
    }

    public async Task<List<(string Value, string Label)>> OptionsAsync(AppDbContext db, FieldDef field, string? current)
    {
        if (field.Options is not null)
        {
            return field.Options.ToList();
        }

        var source = field.Kind == FieldKind.Image ? "media" : field.OptionsSource;
        return source is null ? new() : await OptionSources.LoadAsync(db, source, current);
    }

    // ---------- Helpers ----------

    private static string ToText(object? value) => value switch
    {
        null => string.Empty,
        bool b => b ? "true" : "false",
        List<string> lines => string.Join('\n', lines),
        DateOnly d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        _ => System.Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty,
    };

    public static string Slugify(string text)
    {
        var slug = SlugCleaner.Replace(text.ToLowerInvariant(), "-").Trim('-');
        return slug.Length > 80 ? slug[..80].TrimEnd('-') : slug;
    }
}
