using System.Globalization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OptimumEarth.Web.Services;

/// <summary>
/// Spam traps for public enquiry forms, alongside reCAPTCHA. Pages/Shared/_SpamTrap.cshtml adds two
/// hidden fields, and every POST to the page is checked before its handler runs:
/// <list type="bullet">
/// <item><b>Honeypot.</b> A text field people never see or reach, so only a bot fills it in. The
/// request is marked as caught and the handler skips saving, but still answers as if the enquiry
/// was sent, so the bot is not told what gave it away.</item>
/// <item><b>Timing.</b> An encrypted stamp of when the form was shown. A form sent sooner than
/// <see cref="MinimumFillTime"/> after that, or without a readable stamp (a bot posting straight to
/// the page), gets a model error, so the handler's <c>ModelState.IsValid</c> test refuses it and the
/// page asks the visitor to send again. Refused openly rather than silently, so a quick person
/// never loses an enquiry without knowing.</item>
/// </list>
/// Nothing is stored or emailed either way.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public sealed class SpamTrapAttribute : Attribute, IAsyncPageFilter
{
    /// <summary>
    /// The honeypot field's name: one a form-filling bot will want to complete. It is not a field
    /// browsers or password managers fill in by themselves, and no model on these pages has it.
    /// </summary>
    public const string FieldName = "website";

    /// <summary>The hidden field carrying the encrypted time the form was shown.</summary>
    public const string StampField = "form-shown";

    /// <summary>Where the timing message goes; _SpamTrap.cshtml shows it in the form.</summary>
    public const string ModelStateKey = "SpamTrap";

    /// <summary>
    /// Faster than anyone types even the short enquiry form, with browser autofill: bots post
    /// within a second. Someone who is quicker is asked to send again, not lost.
    /// </summary>
    public static readonly TimeSpan MinimumFillTime = TimeSpan.FromSeconds(3);

    public const string TooFastMessage = "That was quicker than we expected. Please check your details and send again.";

    public const string NoStampMessage = "This form has expired. Please send again.";

    private const string Purpose = "OptimumEarth.Web.SpamTrap.FormShown";

    private static readonly object CaughtKey = new();

    /// <summary>True when the current request filled in the honeypot: the handler must not save it.</summary>
    public static bool Caught(HttpContext context) => context.Items.ContainsKey(CaughtKey);

    /// <summary>The value of <see cref="StampField"/> for a form shown at <paramref name="shown"/>.</summary>
    public static string CreateStamp(IDataProtectionProvider protection, DateTimeOffset shown) =>
        protection.CreateProtector(Purpose).Protect(shown.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture));

    /// <summary>When the form was shown, or null when the stamp is missing, forged or unreadable.</summary>
    public static DateTimeOffset? ReadStamp(IDataProtectionProvider protection, string? stamp)
    {
        if (string.IsNullOrWhiteSpace(stamp))
        {
            return null;
        }

        try
        {
            var text = protection.CreateProtector(Purpose).Unprotect(stamp);
            return long.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var ms)
                ? DateTimeOffset.FromUnixTimeMilliseconds(ms)
                : null;
        }
        catch (System.Security.Cryptography.CryptographicException)
        {
            return null;
        }
    }

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context) => Task.CompletedTask;

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        var http = context.HttpContext;
        if (HttpMethods.IsPost(http.Request.Method) && http.Request.HasFormContentType)
        {
            var logger = http.RequestServices.GetRequiredService<ILogger<SpamTrapAttribute>>();
            var form = http.Request.Form;
            if (!string.IsNullOrWhiteSpace(form[FieldName].ToString()))
            {
                // Answered like a real enquiry, so no timing message that would give the trap away.
                http.Items[CaughtKey] = true;
                logger.LogInformation("Spam trap caught a form submission to {Path}; it was not saved.", http.Request.Path);
            }
            else
            {
                var protection = http.RequestServices.GetRequiredService<IDataProtectionProvider>();
                var shown = ReadStamp(protection, form[StampField].ToString());
                var now = DateTimeOffset.UtcNow;
                if (shown is null || shown > now.AddMinutes(1))
                {
                    context.ModelState.AddModelError(ModelStateKey, NoStampMessage);
                    logger.LogInformation("Spam trap refused a form submission to {Path}: no valid form stamp.", http.Request.Path);
                }
                else if (now - shown < MinimumFillTime)
                {
                    context.ModelState.AddModelError(ModelStateKey, TooFastMessage);
                    logger.LogInformation("Spam trap refused a form submission to {Path}: sent {Seconds:0.0}s after the form was shown.", http.Request.Path, (now - shown.Value).TotalSeconds);
                }
            }
        }

        await next();
    }
}
