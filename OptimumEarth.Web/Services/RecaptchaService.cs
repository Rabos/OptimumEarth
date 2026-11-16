using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace OptimumEarth.Web.Services;

/// <summary>
/// Google reCAPTCHA v2 Invisible keys, bound from the <c>Recaptcha</c> section.
/// The site key is public and is written into the page; the secret key is not and must
/// come from the environment (<c>Recaptcha__SecretKey</c>) or appsettings.Production.json.
/// </summary>
public sealed class RecaptchaOptions
{
    public const string Section = "Recaptcha";

    public string SiteKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    /// <summary>The check runs only when both keys are set, so a missing secret never blocks every enquiry.</summary>
    public bool IsEnabled => !string.IsNullOrWhiteSpace(SiteKey) && !string.IsNullOrWhiteSpace(SecretKey);
}

public enum RecaptchaResult
{
    /// <summary>Keys are not configured; the form is accepted without a check.</summary>
    Disabled,
    Passed,
    /// <summary>No token was sent: the check never ran (JavaScript off or Google's script blocked), or a bot posted directly.</summary>
    Missing,
    /// <summary>Google rejected the token: expired, reused, forged or for another site.</summary>
    Failed,
    /// <summary>Google could not be reached or answered with something unreadable.</summary>
    Unavailable,
}

/// <summary>Checks a reCAPTCHA v2 token with Google's siteverify endpoint (the same for the checkbox and Invisible widgets).</summary>
public sealed class RecaptchaService
{
    /// <summary>The form field Google's widget fills in with the token.</summary>
    public const string ResponseField = "g-recaptcha-response";

    public const string VerifyUrl = "https://www.google.com/recaptcha/api/siteverify";

    private readonly HttpClient _http;
    private readonly RecaptchaOptions _options;
    private readonly ILogger<RecaptchaService> _logger;

    public RecaptchaService(HttpClient http, IOptions<RecaptchaOptions> options, ILogger<RecaptchaService> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<RecaptchaResult> VerifyAsync(string? token, CancellationToken cancellationToken = default)
    {
        if (!_options.IsEnabled)
        {
            return RecaptchaResult.Disabled;
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return RecaptchaResult.Missing;
        }

        // The visitor's IP is optional and, behind the reverse proxy, would only be the proxy's, so it is not sent.
        using var body = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["secret"] = _options.SecretKey,
            ["response"] = token,
        });

        SiteVerifyResponse? reply;
        try
        {
            using var response = await _http.PostAsync(VerifyUrl, body, cancellationToken);
            response.EnsureSuccessStatusCode();
            reply = await response.Content.ReadFromJsonAsync<SiteVerifyResponse>(cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException or NotSupportedException)
        {
            _logger.LogWarning("reCAPTCHA verification could not reach Google: {Error}", ex.Message);
            return RecaptchaResult.Unavailable;
        }

        if (reply is null)
        {
            _logger.LogWarning("reCAPTCHA verification returned an empty response.");
            return RecaptchaResult.Unavailable;
        }

        if (reply.Success)
        {
            return RecaptchaResult.Passed;
        }

        var codes = reply.ErrorCodes ?? Array.Empty<string>();
        // These mean the server's own keys are wrong, not that the visitor is a bot.
        if (codes.Contains("invalid-input-secret") || codes.Contains("missing-input-secret"))
        {
            _logger.LogError("reCAPTCHA rejected the secret key ({Codes}). Check Recaptcha:SecretKey.", string.Join(", ", codes));
        }
        else
        {
            _logger.LogInformation("reCAPTCHA check failed for a form submission ({Codes}).", string.Join(", ", codes));
        }

        return RecaptchaResult.Failed;
    }

    /// <summary>What the visitor is told when the check does not pass; null when it did (or is switched off).</summary>
    public static string? ErrorMessage(RecaptchaResult result) => result switch
    {
        RecaptchaResult.Missing => "Our anti-spam check didn't run. Please make sure JavaScript is switched on, then send again.",
        RecaptchaResult.Failed => "We couldn't confirm you're not a robot. Please send again.",
        RecaptchaResult.Unavailable => "We couldn't run our anti-spam check just now. Please try again in a moment.",
        _ => null,
    };

    private sealed class SiteVerifyResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error-codes")]
        public string[]? ErrorCodes { get; set; }
    }
}
