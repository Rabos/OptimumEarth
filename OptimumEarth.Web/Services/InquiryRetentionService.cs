using Microsoft.EntityFrameworkCore;
using OptimumEarth.Web.Data;

namespace OptimumEarth.Web.Services;

/// <summary>
/// Enquiries hold names, emails and phone numbers. Once a day, delete the ones
/// older than the retention period set in Site settings (0 keeps them forever).
/// </summary>
public sealed class InquiryRetentionService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<InquiryRetentionService> _logger;

    public InquiryRetentionService(IServiceProvider services, ILogger<InquiryRetentionService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Give startup (migrations, seeding) time to finish first.
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var months = await db.Settings.Select(s => s.RetentionMonths).FirstOrDefaultAsync(stoppingToken);
                if (months > 0)
                {
                    var cutoff = DateTime.UtcNow.AddMonths(-months);
                    var removed = await db.Inquiries.Where(i => i.CreatedUtc < cutoff).ExecuteDeleteAsync(stoppingToken);
                    if (removed > 0)
                    {
                        _logger.LogInformation("Deleted {Count} enquiries older than {Months} months.", removed, months);
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Enquiry retention run failed.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
