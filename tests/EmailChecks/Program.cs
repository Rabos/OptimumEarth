using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using OptimumEarth.Web.Areas.Admin.Pages;
using OptimumEarth.Web.Data;
using OptimumEarth.Web.Models;
using OptimumEarth.Web.Services;

var checks = 0;
void Check(bool condition, string name)
{
    if (!condition) throw new Exception("Failed: " + name);
    checks++;
}

var settings = new SiteSettings { UgandaTo = "ug@example.com", ZambiaTo = "zm@example.com", FoundationTo = "foundation@example.com" };
foreach (var destination in InquiryEmailService.Destinations)
    Check(InquiryEmailService.Route(destination, "Foundation") == destination, destination + " uses its own mailbox");
Check(InquiryEmailService.Route("Not sure yet", "Zambia") == "Zambia", "Unknown-country selection uses configured default");
Check(InquiryEmailService.Route("Not sure yet", "invalid") == "Uganda", "Invalid default safely routes to Uganda");
Check(new ContactInquiry { Audience = "Foundation supporter", Country = "Zambia" }.Destination == "Foundation", "Foundation audience takes priority over country");
Check(new ContactInquiry { Audience = "Client", Country = "Zambia" }.Destination == "Zambia", "Contact clients route by country");
Check(InquiryEmailService.Recipient(settings, "Foundation") == "foundation@example.com", "Recipient follows destination");

using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
    .UseNpgsql("Host=localhost;Database=unused;Username=unused;Password=unused").Options);
var protection = new EphemeralDataProtectionProvider();
var sender = new InquiryEmailService(db, protection);
const string password = "test-only-password-123";
var protectedPassword = sender.ProtectPassword(password);
Check(!protectedPassword.Contains(password), "Password is encrypted");
Check(protection.CreateProtector("OptimumEarth.SmtpPasswords.v1").Unprotect(protectedPassword) == password, "Saved password can be decrypted");
Check(!InquiryEmailService.Error(new Exception(password)).Contains(password), "Delivery errors do not expose exception secrets");
Check(InquiryEmailService.Error(new EmailNotConfiguredException()).Contains("Set up"), "Missing settings have actionable status");
var invalid = new EmailModel.MailboxInput { Destination = "Uganda", Address = "bad", Recipient = "bad" };
Check(!Validator.TryValidateObject(invalid, new ValidationContext(invalid), new List<ValidationResult>(), true), "Mailbox addresses are validated");
Check(new Inquiry().EmailDelivery == EmailDeliveryStatus.NotQueued, "Old enquiries are not automatically sent");
var migrationSql = db.GetService<IMigrator>().GenerateScript("20261003120000_AddCountryPageHeroImagePath", "20261007061641_AddHostingerEmail");
Check(migrationSql.Contains("CREATE TABLE \"EmailMailboxes\""), "Migration creates mailbox storage");
Check(migrationSql.Contains("DEFAULT 'Uganda'"), "Existing settings get a valid default destination");
Check(migrationSql.Contains("\"EmailDelivery\" integer NOT NULL DEFAULT 0"), "Existing enquiries remain unqueued after migration");
Console.WriteLine($"Passed {checks} email configuration, routing, encryption and migration checks. No SMTP messages sent and no database connection used.");
