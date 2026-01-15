using DailySpendServer.Data;
using Microsoft.EntityFrameworkCore;

namespace DailySpendServer.Services.Background;

public class WebHookRecoveryJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public WebHookRecoveryJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var _db = scope.ServiceProvider.GetRequiredService<DailySpendContext>();
            var _hookService = scope.ServiceProvider.GetRequiredService<WebHookService>();
            var webhookBrokenUsers = await _db.UserSettings
                .Where(u => u.IsActive && (!u.WebHookActive || u.LastWebhookReceivedAt < DateTime.UtcNow.AddHours(-24))).ToListAsync(ct);
            foreach (var user in webhookBrokenUsers)
            {
                try
                {
                    await _hookService.RegisterWebHook(user.id);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(10),ct);
        }
    }
}