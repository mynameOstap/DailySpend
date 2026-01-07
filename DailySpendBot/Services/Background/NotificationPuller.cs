using DailySpendBot.DTO;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace DailySpendBot.Services.Background
{
    public class NotificationPuller : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly BotHttpClient _httpClient;
        public NotificationPuller(ITelegramBotClient botClient, BotHttpClient httpClient)
        {
            _botClient = botClient;
            _httpClient = httpClient;
        }
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try 
                {
                    var notifications = await _httpClient.GetAsync<List<NotificationDTO>>("api/monobank/notification");
                    foreach  (var n in notifications)
                    {
                        if (string.IsNullOrEmpty(n.ChatId))
                            continue;

                        var amount = n.Amount / 100m;
                        var balance = n.Balance / 100m;

                        var msg = new StringBuilder()
                            .AppendLine("💸 <b>New Transaction</b>")
                            .AppendLine(!string.IsNullOrWhiteSpace(n.Description) ? $"📝 {n.Description!}" : "")
                            .AppendLine($"💰 Amount: {amount:0.00} UAH")
                            .AppendLine($"🏦 Balance: {balance:0.00} UAH")
                            .ToString();

                        await _botClient.SendMessage(
                            chatId: n.ChatId,
                            text: msg,
                            parseMode: ParseMode.Html,
                            cancellationToken: ct
                        );

                        var ok = await _httpClient.MarkNotification($"api/monobank/notification/mark/{n.id}");
                        if (!ok)
                            Console.WriteLine($"Failed to mark notification {n.id}");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"NotificationPuller error: {ex}");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), ct);
            }
        }

    }
}
