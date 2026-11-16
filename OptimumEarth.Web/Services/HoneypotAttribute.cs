using Microsoft.AspNetCore.Mvc.Filters;

namespace OptimumEarth.Web.Services;

/// <summary>
/// A spam trap for public enquiry forms, alongside reCAPTCHA. Pages/Shared/_Honeypot.cshtml adds a
/// text field that people never see or reach, so only a bot fills it in. Every POST to the page is
/// checked before its handler runs; when the field has a value the request is marked as caught and
/// the handler skips saving, but still answers as if the enquiry was sent, so the bot is not told
/// what gave it away. Nothing is stored or emailed.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public sealed class HoneypotAttribute : Attribute, IAsyncPageFilter
{
    /// <summary>
    /// The trap field's name: one a form-filling bot will want to complete. It is not a field
    /// browsers or password managers fill in by themselves, and no model on these pages has it.
    /// </summary>
    public const string FieldName = "website";

    private static readonly object CaughtKey = new();

    /// <summary>True when the current request filled in the trap: the handler must not save it.</summary>
    public static bool Caught(HttpContext context) => context.Items.ContainsKey(CaughtKey);

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context) => Task.CompletedTask;

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        var http = context.HttpContext;
        if (HttpMethods.IsPost(http.Request.Method)
            && http.Request.HasFormContentType
            && !string.IsNullOrWhiteSpace(http.Request.Form[FieldName].ToString()))
        {
            http.Items[CaughtKey] = true;
            http.RequestServices.GetRequiredService<ILogger<HoneypotAttribute>>()
                .LogInformation("Spam trap caught a form submission to {Path}; it was not saved.", http.Request.Path);
        }

        await next();
    }
}
