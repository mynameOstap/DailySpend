using DailySpendBot.DTO;
using DailySpendBot.Enums;
using DailySpendBot.Messages;
using DailySpendBot.Sessions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace DailySpendBot.Services.Background;

public class UserEventMessagePuller : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly BotSessionStore _session;

    public UserEventMessagePuller(ITelegramBotClient botClient, IServiceScopeFactory scopeFactory,
        BotSessionStore session)
    {
        _botClient = botClient;
        _scopeFactory = scopeFactory;
        _session = session;
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
                var eventMessage = await httpClient.GetAsync<List<UserEventMessageDTO>>("api/user/notification");
                foreach (var m in eventMessage)
                {
                    if (string.IsNullOrEmpty(m.ChatId))
                    {
                        continue;
                    }

                    var msg = UserEventMessageBuilder.Build(m);
                    await _botClient.SendMessage(m.ChatId, msg, ParseMode.Html);
                    await HandleEventAsync(m);
                    var ok = await httpClient.MarkNotification(
                        $"api/user/notification/mark/{m.Id}");

                    if (!ok)
                        Console.WriteLine(
                            $"Failed to mark notification {m.Id}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), ct);
        }
    }

    private async Task HandleEventAsync(UserEventMessageDTO messageDto)
    {
        var chatId = long.Parse(messageDto.ChatId);
        var userId = long.Parse(messageDto.UserId);
        switch (messageDto.Type)
        {
            case UserEventType.ResetSetting:
                _session.Clear(userId);

                var s = _session.GetOrCreate(userId);

                s.Step = PendingInput.Token;

                await _botClient.SendMessage(
                    chatId,
                    "⚠️ Ваші налаштування скинуто (або термін дії вийшов).\n\n" +
                    "Давайте налаштуємо бота заново. \n" +
                    "Будь ласка, **введіть ваш токен Monobank**:",
                    parseMode: ParseMode.Markdown
                );
                break;
        }
    }
}