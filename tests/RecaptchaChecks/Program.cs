using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OptimumEarth.Web.Services;

var checks = 0;
void Check(bool condition, string name)
{
    if (!condition) throw new Exception("Failed: " + name);
    checks++;
}

const string siteKey = "site-key";
const string secretKey = "test-only-secret";

RecaptchaService Service(FakeGoogle google, string secret = secretKey) =>
    new(new HttpClient(google), Options.Create(new RecaptchaOptions { SiteKey = siteKey, SecretKey = secret }), NullLogger<RecaptchaService>.Instance);

// Switched off until both keys are set, so a missing secret never blocks every enquiry.
var google = FakeGoogle.Json("""{"success":true}""");
Check(await Service(google, secret: "").VerifyAsync("token") == RecaptchaResult.Disabled, "No secret key turns the check off");
Check(!new RecaptchaOptions { SiteKey = "", SecretKey = secretKey }.IsEnabled, "No site key turns the check off");
Check(new RecaptchaOptions { SiteKey = siteKey, SecretKey = secretKey }.IsEnabled, "Both keys turn the check on");
Check(google.Calls == 0, "Google is not called while the check is off");

// An unticked box never reaches Google.
Check(await Service(google).VerifyAsync(null) == RecaptchaResult.Missing, "No token is refused");
Check(await Service(google).VerifyAsync("  ") == RecaptchaResult.Missing, "Blank token is refused");
Check(google.Calls == 0, "Google is not called without a token");

// Google accepts the token.
Check(await Service(google).VerifyAsync("good-token") == RecaptchaResult.Passed, "Accepted token passes");
Check(google.Calls == 1, "Google is called once per submission");
Check(google.LastUrl == RecaptchaService.VerifyUrl, "Token is posted to siteverify");
Check(google.LastBody.Contains("secret=" + secretKey) && google.LastBody.Contains("response=good-token"), "Secret and token are both sent");
Check(!google.LastBody.Contains("remoteip"), "Proxy IP is not sent as the visitor's");

// Google rejects the token.
Check(await Service(FakeGoogle.Json("""{"success":false,"error-codes":["timeout-or-duplicate"]}""")).VerifyAsync("reused") == RecaptchaResult.Failed, "Expired or reused token fails");
Check(await Service(FakeGoogle.Json("""{"success":false,"error-codes":["invalid-input-secret"]}""")).VerifyAsync("t") == RecaptchaResult.Failed, "Wrong secret key fails closed");
Check(await Service(FakeGoogle.Json("""{"success":false}""")).VerifyAsync("t") == RecaptchaResult.Failed, "Rejection without error codes fails");

// Google unreachable or answering nonsense: refuse rather than let bots through.
Check(await Service(FakeGoogle.Status(HttpStatusCode.InternalServerError)).VerifyAsync("t") == RecaptchaResult.Unavailable, "Google error status is refused");
Check(await Service(FakeGoogle.Json("<html>not json</html>")).VerifyAsync("t") == RecaptchaResult.Unavailable, "Unreadable reply is refused");
Check(await Service(FakeGoogle.Json("null")).VerifyAsync("t") == RecaptchaResult.Unavailable, "Empty reply is refused");
Check(await Service(FakeGoogle.Throws(new HttpRequestException("no route"))).VerifyAsync("t") == RecaptchaResult.Unavailable, "Network failure is refused");
Check(await Service(FakeGoogle.Throws(new TaskCanceledException("timeout"))).VerifyAsync("t") == RecaptchaResult.Unavailable, "Timeout is refused");

// Only a pass (or a switched-off check) lets the enquiry through.
Check(RecaptchaService.ErrorMessage(RecaptchaResult.Passed) is null, "Pass has no message");
Check(RecaptchaService.ErrorMessage(RecaptchaResult.Disabled) is null, "Switched-off check has no message");
foreach (var refused in new[] { RecaptchaResult.Missing, RecaptchaResult.Failed, RecaptchaResult.Unavailable })
    Check(!string.IsNullOrWhiteSpace(RecaptchaService.ErrorMessage(refused)), refused + " tells the visitor what to do");

// Every public enquiry form is protected.
foreach (var page in new[] { typeof(OptimumEarth.Web.Pages.ContactModel), typeof(OptimumEarth.Web.Pages.FoundationModel), typeof(OptimumEarth.Web.Pages.UgandaModel), typeof(OptimumEarth.Web.Pages.ZambiaModel) })
    Check(page.GetCustomAttributes(typeof(RequireRecaptchaAttribute), inherit: true).Length == 1, page.Name + " requires reCAPTCHA");

Console.WriteLine($"Passed {checks} reCAPTCHA checks. No request sent to Google.");

sealed class FakeGoogle : HttpMessageHandler
{
    private readonly Func<HttpResponseMessage> _reply;

    private FakeGoogle(Func<HttpResponseMessage> reply) => _reply = reply;

    public int Calls { get; private set; }
    public string LastUrl { get; private set; } = "";
    public string LastBody { get; private set; } = "";

    public static FakeGoogle Json(string json) => new(() => new HttpResponseMessage(HttpStatusCode.OK)
    {
        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"),
    });

    public static FakeGoogle Status(HttpStatusCode status) => new(() => new HttpResponseMessage(status));

    public static FakeGoogle Throws(Exception ex) => new(() => throw ex);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Calls++;
        LastUrl = request.RequestUri?.ToString() ?? "";
        LastBody = request.Content is null ? "" : await request.Content.ReadAsStringAsync(cancellationToken);
        return _reply();
    }
}
