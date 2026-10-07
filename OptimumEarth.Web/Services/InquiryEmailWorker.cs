using Microsoft.EntityFrameworkCore;
using OptimumEarth.Web.Data;

namespace OptimumEarth.Web.Services;

/// <summary>The enquiries table is a durable queue; public forms only need to commit their enquiry.</summary>
public sealed class InquiryEmailWorker(IServiceScopeFactory scopes, ILogger<InquiryEmailWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopes.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                // A restart during delivery has an uncertain outcome; require a deliberate retry.
                var cutoff = DateTime.UtcNow.AddMinutes(-5);
                await db.Inquiries.Where(i => i.EmailDelivery == EmailDeliveryStatus.Sending && i.EmailAttemptUtc < cutoff)
                    .ExecuteUpdateAsync(s => s.SetProperty(i => i.EmailDelivery, EmailDeliveryStatus.Failed)
                        .SetProperty(i => i.EmailDeliveryError, "Delivery was interrupted; check the inbox before retrying."), stoppingToken);
                var ids = await db.Inquiries.Where(i => i.EmailDelivery == EmailDeliveryStatus.Pending)
                    .OrderBy(i => i.CreatedUtc).Select(i => i.Id).Take(10).ToListAsync(stoppingToken);
                foreach (var id in ids)
                {
                    var claimed = await db.Inquiries.Where(i => i.Id == id && i.EmailDelivery == EmailDeliveryStatus.Pending)
                        .ExecuteUpdateAsync(s => s.SetProperty(i => i.EmailDelivery, EmailDeliveryStatus.Sending)
                            .SetProperty(i => i.EmailAttemptUtc, DateTime.UtcNow), stoppingToken);
                    if (claimed == 0) continue;
                    var inquiry = await db.Inquiries.SingleAsync(i => i.Id == id, stoppingToken);
                    try
                    {
                        var sender = scope.ServiceProvider.GetRequiredService<InquiryEmailService>();
                        await sender.SendAsync(inquiry.Destination, $"Website enquiry #{inquiry.Id} — {inquiry.Destination}",
                            InquiryEmailService.Body(inquiry), inquiry.Email, stoppingToken);
                        inquiry.EmailDelivery = EmailDeliveryStatus.Sent;
                        inquiry.EmailSentUtc = DateTime.UtcNow;
                        inquiry.EmailDeliveryError = string.Empty;
                    }
                    catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                    {
                        inquiry.EmailDelivery = ex is EmailNotConfiguredException ? EmailDeliveryStatus.NotConfigured : EmailDeliveryStatus.Failed;
                        inquiry.EmailDeliveryError = InquiryEmailService.Error(ex);
                        logger.LogWarning("Email for enquiry {Id} failed ({ErrorType}).", id, ex.GetType().Name);
                    }
                    await db.SaveChangesAsync(stoppingToken);
                    db.ChangeTracker.Clear();
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception ex)
            {
                logger.LogWarning("Enquiry email queue unavailable ({ErrorType}).", ex.GetType().Name);
            }
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
