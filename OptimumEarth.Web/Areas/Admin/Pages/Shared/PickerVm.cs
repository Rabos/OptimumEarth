using OptimumEarth.Web.Areas.Admin.Content;

namespace OptimumEarth.Web.Areas.Admin.Pages;

public sealed record PickerVm(FieldDef Def, PickerData Data, string? Error, bool ReadOnly);
