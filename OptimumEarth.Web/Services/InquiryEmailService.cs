using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using OptimumEarth.Web.Data;

namespace OptimumEarth.Web.Services;

public sealed class InquiryEmailService(AppDbContext db, IDataProtectionProvider protection)
{
    public static readonly string[] Destinations = ["Uganda", "Zambia", "Foundation"];
    private readonly IDataProtector _protector = protection.CreateProtector("OptimumEarth.SmtpPasswords.v1");

    public string ProtectPassword(string password) => _protector.Protect(password);

    public static string Route(string destination, string fallback) =>
        Destinations.Contains(destination) ? destination : Destinations.Contains(fallback) ? fallback : "Uganda";

    public static string Recipient(SiteSettings settings, string destination) => destination switch
    {
        "Zambia" => settings.ZambiaTo,
        "Foundation" => settings.FoundationTo,
        _ => settings.UgandaTo,
    };

    public async Task SendAsync(string destination, string subject, string body, string? replyTo, CancellationToken ct)
    {
        var settings = await db.Settings.AsNoTracking().FirstAsync(ct);
        var route = Route(destination, settings.DefaultEmailDestination);
        var mailbox = await db.EmailMailboxes.AsNoTracking().SingleOrDefaultAsync(m => m.Destination == route, ct);
        if (mailbox is null || string.IsNullOrWhiteSpace(mailbox.Address) || string.IsNullOrEmpty(mailbox.ProtectedPassword))
            throw new EmailNotConfiguredException();

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Optimum Earth " + route, mailbox.Address));
        message.To.Add(MailboxAddress.Parse(Recipient(settings, route)));
        if (!string.IsNullOrWhiteSpace(replyTo)) message.ReplyTo.Add(MailboxAddress.Parse(replyTo));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(30));
        using var smtp = new SmtpClient { Timeout = 30000 };
        await smtp.ConnectAsync("smtp.hostinger.com", 465, SecureSocketOptions.SslOnConnect, timeout.Token);
        await smtp.AuthenticateAsync(mailbox.Address, _protector.Unprotect(mailbox.ProtectedPassword), timeout.Token);
        await smtp.SendAsync(message, timeout.Token);
        // An accepted message remains sent even if the server closes during QUIT.
        try { await smtp.DisconnectAsync(true, timeout.Token); }
        catch (Exception) { }
    }

    public static string Body(Inquiry inquiry) => $"""
        Website enquiry #{inquiry.Id}
        Received: {inquiry.CreatedUtc:u}
        Form: {inquiry.Source}
        Destination: {inquiry.Destination}
        Name: {inquiry.FullName}
        Organisation: {inquiry.Organisation}
        Email: {inquiry.Email}
        Phone: {inquiry.Phone}
        Audience: {inquiry.Audience}
        Country: {inquiry.Country}
        Service: {inquiry.Service}

        {inquiry.Message}
        """;

    // Do not persist SMTP exception messages: they can contain mailbox or authentication details.
    public static string Error(Exception exception) => exception switch
    {
        EmailNotConfiguredException => "Set up this destination's mailbox in Email configuration, then retry.",
        MailKit.Security.AuthenticationException => "SMTP authentication failed. Check the mailbox address and password.",
        System.Security.Cryptography.CryptographicException => "The saved password could not be decrypted. Enter it again in Email configuration.",
        OperationCanceledException => "The SMTP connection timed out. Check the configuration and retry.",
        _ => "Email delivery failed. Check the mailbox, recipient and SMTP connection, then retry.",
    };
}

public sealed class EmailNotConfiguredException : Exception;
