using System.Text.Json;
using System.Text.Json.Serialization;
using DailySpendBot.DTO;
using DailySpendBot.Services;
using DailySpendBot.Sessions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DailySpendBot.Handlers
{
    public class CallBackHandler
    {
        private readonly BotHttpClient _botHttpClient;
        private readonly BotSessionStore _sessions;

        public CallBackHandler(BotHttpClient botHttpClient, BotSessionStore sessions)
        {
            _botHttpClient = botHttpClient;
            _sessions = sessions;
        }

        public async Task Handle(ITelegramBotClient bot, CallbackQuery cq, CancellationToken ct = default)
        {
            var userId = cq.From.Id;
            var chatId = cq.Message!.Chat.Id;
            var data = cq.Data ?? "";
            var s = _sessions.GetOrCreate(userId);

            if (data.StartsWith("acc:"))
            {
                var accountId = data["acc:".Length..];
                var account = s.Accounts.FirstOrDefault(a => a.id == accountId);
                s.SelectedAccountId = accountId;
                s.maskedPan = account.maskedPan.FirstOrDefault() ?? "";
                s.balance = account.Balance;
                

                if (s.Goal is null)
                {
                    s.Step = PendingInput.Goal;
                    await bot.SendMessage(chatId, "✅ Картку вибрано!\n\nВведи ціль (число), напр. 500:", cancellationToken: ct);
                }

                await bot.AnswerCallbackQuery(cq.Id, cancellationToken: ct);
                return;
            }

            switch (data)
            {
                case "menu:main":
                    s.Step = PendingInput.None;
                    await bot.SendMessage(chatId, "Головне меню", replyMarkup: Menus.MainReply(), cancellationToken: ct);
                    break;

                case "token:retry":
                    s.Step = PendingInput.Token;
                    s.Token = null;
                    s.Accounts?.Clear();
                    s.SelectedAccountId = null;
                    s.balance = 0;
                    s.maskedPan = string.Empty;
                    

                    await bot.SendMessage(chatId, "Введи токен ще раз:", replyMarkup: Menus.SettingsReply(), cancellationToken: ct);
                    break;

                case "confirm":
                    if (string.IsNullOrWhiteSpace(s.Token) || s.Goal is null || s.Days is null || string.IsNullOrWhiteSpace(s.SelectedAccountId))
                    {
                        await bot.SendMessage(chatId, "Немає всіх даних для підтвердження. Пройди налаштування ще раз.", replyMarkup: Menus.MainReply(), cancellationToken: ct);
                        break;
                    }

                    await _botHttpClient.CreateUser(new UserSettingDTO
                    {
                        id = userId.ToString(),
                        ChatId = chatId.ToString(),
                        token = s.Token,
                        GoalAmount = s.Goal.Value,
                        daysToSalary = s.Days.Value,
                        SelectedAccountId = s.SelectedAccountId,
                        maskedPan = s.maskedPan,
                        balance = s.balance
                    });

                    _sessions.Clear(userId);
                    await bot.SendMessage(chatId, "✅ Налаштування збережено!", replyMarkup: Menus.MainReply(), cancellationToken: ct);
                    break;

                case "cancel":
                    s.Step = PendingInput.Token;
                    s.Token = null;
                    s.Goal = null;
                    s.Accounts?.Clear();
                    await bot.SendMessage(chatId,"Введіть токен", cancellationToken: ct);
                    break;

                default:
                    await bot.SendMessage(chatId, $"Невідома дія: {data}", cancellationToken: ct);
                    break;
            }

            await bot.AnswerCallbackQuery(cq.Id, cancellationToken: ct);
        }

    
    }
}
