using DailySpendBot.DTO;
using Microsoft.Extensions.Hosting;

using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.DependencyInjection;


namespace DailySpendBot.Services.Background
{
    public class NotificationPuller : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IServiceScopeFactory _scopeFactory;

    public NotificationPuller(
        ITelegramBotClient botClient,
        IServiceScopeFactory scopeFactory)
    {
        _botClient = botClient;
        _scopeFactory = scopeFactory;
    }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var httpClient = scope.ServiceProvider
                        .GetRequiredService<BotHttpClient>();

                    var notifications =
                        await httpClient.GetAsync<List<NotificationDTO>>(
                            "api/monobank/notification");

                    foreach (var n in notifications)
                    {
                        if (string.IsNullOrEmpty(n.ChatId))
                            continue;

                        var msg = BuildMessage(n);

                        await _botClient.SendMessage(
                            chatId: n.ChatId,
                            text: msg,
                            parseMode: ParseMode.Html,
                            cancellationToken: ct);

                        var ok = await httpClient.MarkNotification(
                            $"api/monobank/notification/mark/{n.id}");

                        if (!ok)
                            Console.WriteLine(
                                $"Failed to mark notification {n.id}");
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"NotificationPuller error: {ex}");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), ct);
            }
        }

        private static string BuildMessage(NotificationDTO n)
        {
            var amount = n.Amount / 100m;
            var balance = n.Balance / 100m;

            return new StringBuilder()
                .AppendLine("💸 <b>New Transaction</b>")
                .AppendLine(!string.IsNullOrWhiteSpace(n.Description)
                    ? $"📝 {n.Description}"
                    : "")
                .AppendLine($"💰 Amount: {amount:0.00} UAH")
                .AppendLine($"🏦 Balance: {balance:0.00} UAH")
                .ToString();
        }
}

}
