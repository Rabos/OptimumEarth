using OptimumEarth.Web.Areas.Admin.Content;

namespace OptimumEarth.Web.Areas.Admin.Pages;

public sealed record FieldVm(
    FieldDef Def,
    string Value,
    string? Error,
    IReadOnlyList<(string Value, string Label)> Options,
    IReadOnlyList<PlacementOption> Placements,
    bool ReadOnly,
    PickerData? Picker = null);
