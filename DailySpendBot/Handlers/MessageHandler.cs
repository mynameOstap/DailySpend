using DailySpendBot.Services;
using DailySpendBot.Sessions;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Globalization;
using Telegram.Bot.Types.ReplyMarkups;
using DailySpendBot.DTO;

namespace DailySpendBot.Handlers
{
    public class MessageHandler
    {
        private readonly BotHttpClient _botHttpClient;
        private readonly BotSessionStore _session;
        private readonly CacheService _cache;

        public MessageHandler(BotHttpClient botHttpClient, BotSessionStore session, CacheService cache)
        {
            _botHttpClient = botHttpClient;
            _session = session;
            _cache = cache;
        }

        public async Task HandleMessage(ITelegramBotClient bot, Message msg, CancellationToken ct = default)
        {
            if (msg.Text is null) return;

            var chatId = msg.Chat.Id;
            var userId = msg.From!.Id;
            var text = msg.Text.Trim();
            string status;
            UserSettingDTO settings;

            var s = _session.GetOrCreate(userId);
            s.id = userId.ToString();

            if (text == "/start")
            {
                s.Step = PendingInput.None;
                await bot.SendMessage(chatId, "Привіт! Обери дію:", replyMarkup: Menus.MainReply(), cancellationToken: ct);
                return;
            }

            if (text == Menus.BtnStatus)
            {
                if (!string.IsNullOrEmpty(_cache.GetUserStatus(userId.ToString())))
                {
                    status = _cache.GetUserStatus(userId.ToString());
                    await bot.SendMessage(chatId, status, replyMarkup: Menus.MainReply(), cancellationToken: ct);
                    return;
                }
                status = await _botHttpClient.GetUserStatus(userId.ToString());
                if (string.IsNullOrEmpty(status) || status == "null")
                {
                    await bot.SendMessage(chatId, "Ви ще не налаштували бота. Перейдіть в Налаштування.", replyMarkup: Menus.MainReply(), cancellationToken: ct);
                    return;
                }
                _cache.SetUserStatus(userId.ToString(), status);
                await bot.SendMessage(chatId, status, replyMarkup: Menus.MainReply(), cancellationToken: ct);
                return;
            }

            if (text == Menus.BtnSettings)
            {
                settings = await _botHttpClient.GetUserSettings(userId.ToString());
                if (settings == null)
                {
                    s.Step = PendingInput.Token;

                    //var path = Path.Combine(AppContext.BaseDirectory, "assets", "token_help.jpg");
                    //await using var stream = File.OpenRead(path);

                    //await bot.SendPhoto(
                    //    chatId: chatId,
                    //    photo: InputFile.FromStream(stream, "token_help.jpg"),
                    //    caption: "Щоб підключити бота, додай токен.\n\n1/3 Введи токен повідомленням сюди 👇",
                    //    cancellationToken: ct
                    //);
                    await bot.SendMessage(chatId, "Введи токен:",replyMarkup: new ReplyKeyboardRemove() , cancellationToken: ct);
                    return;
                }

                s.Step = PendingInput.None;
                await bot.SendMessage(chatId, "Меню налаштувань:", replyMarkup: Menus.SettingsReply(), cancellationToken: ct);
                return;
            }

            if (text == Menus.BtnBackToMain)
            {
                s.Step = PendingInput.None;
                await bot.SendMessage(chatId, "Головне меню", replyMarkup: Menus.MainReply(), cancellationToken: ct);
                return;
            }

            if (text == Menus.BtnSetToken)
            {
                await bot.SendMessage(chatId, $"Ваш токен ", replyMarkup: Menus.SettingsReply(), cancellationToken: ct);
                return;
            }

            if (text == Menus.BtnSetCard)
            {
                if (string.IsNullOrWhiteSpace(s.Token))
                {
                    s.Step = PendingInput.Token;
                    await bot.SendMessage(chatId, "Спочатку введи токен:", replyMarkup: Menus.SettingsReply(), cancellationToken: ct);
                    return;
                }

                var accounts = await _botHttpClient.ValidateToken(s.Token);
                Console.WriteLine(accounts);
                if (accounts == null || accounts.Count == 0)
                {
                    s.Step = PendingInput.Token;
                    await bot.SendMessage(chatId, "Не вдалося отримати картки. Введи токен ще раз:", replyMarkup: Menus.SettingsReply(), cancellationToken: ct);
                    return;
                }

                s.Accounts = accounts;
                s.Step = PendingInput.ChooseCard;

                await bot.SendMessage(chatId, "Обери картку:", replyMarkup: Menus.CardsMenu(s.Accounts), cancellationToken: ct);
                return;
            }

            if (text == Menus.BtnSetGoal)
            {
                s.Step = PendingInput.Goal;
                await bot.SendMessage(chatId, "Введи нову ціль (число), напр. 500:", replyMarkup: Menus.SettingsReply(), cancellationToken: ct);
                return;
            }

            if (text == Menus.BtnSetDays)
            {
                s.Step = PendingInput.Days;
                await bot.SendMessage(chatId, "Введи кількість днів до зарплати (наприклад, 30):", replyMarkup: Menus.SettingsReply(), cancellationToken: ct);
                return;
            }
            await HandleSettingSteps(bot, msg, ct);

        }

        private async Task HandleSettingSteps(ITelegramBotClient bot, Message msg, CancellationToken ct)
        {
            var chatId = msg.Chat.Id;
            var userId = msg.From!.Id;
            var s = _session.GetOrCreate(userId);

            switch (s.Step)
            {
                case PendingInput.Token:
                    {
                        var token = msg.Text!.Trim();
                        var accounts = await _botHttpClient.ValidateToken(token);

                        if (accounts == null || accounts.Count == 0)
                        {
                            s.Step = PendingInput.Token;
                            await bot.SendMessage(chatId, "❌ Невірний токен або рахунків не знайдено. Введи токен ще раз:", cancellationToken: ct);
                            return;
                        }

                        s.Token = token;
                        s.Accounts = accounts;

                        if (s.Accounts.Count == 1)
                        {
                            s.SelectedAccountId = s.Accounts[0].id;

                            if (s.Goal is null)
                            {
                                s.Step = PendingInput.Goal;
                                await bot.SendMessage(chatId, "✅ Токен додано.\nВведи ціль (число), напр. 500:", cancellationToken: ct);
                                return;
                            }
                            if (s.Days is null)
                            {
                                s.Step = PendingInput.Days;
                                await bot.SendMessage(chatId, "✅ Токен додано.\nВведи кількість днів до зарплати:", cancellationToken: ct);
                                return;
                            }

                            s.Step = PendingInput.Review;
                            await bot.SendMessage(chatId, "✅ Токен додано.", cancellationToken: ct);
                            await bot.SendMessage(chatId, BuildSummary(s), replyMarkup: Menus.Review(), cancellationToken: ct);
                            return;
                        }

                        s.Step = PendingInput.ChooseCard;
                        await bot.SendMessage(chatId, "✅ Токен додано! Обери картку:", replyMarkup: Menus.CardsMenu(s.Accounts), cancellationToken: ct);
                        return;
                    }

                case PendingInput.Goal:
                    {
                        var txt = msg.Text!.Trim().Replace(',', '.');
                        if (!int.TryParse(txt, NumberStyles.Any, CultureInfo.InvariantCulture, out var goal) || goal <= 0)
                        {
                            await bot.SendMessage(chatId, "Введи коректну ціль числом (наприклад, 500):", cancellationToken: ct);
                            return;
                        }

                        s.Goal = goal;

                        if (s.Days is null)
                        {
                            s.Step = PendingInput.Days;
                            await bot.SendMessage(chatId, "✅ Ціль збережено. Тепер введи дні до зарплати:", cancellationToken: ct);
                            return;
                        }

                        s.Step = PendingInput.Review;
                        await bot.SendMessage(chatId, BuildSummary(s), replyMarkup: Menus.Review(), cancellationToken: ct);
                        return;
                    }

                case PendingInput.Days:
                    {
                        if (!int.TryParse(msg.Text!.Trim(), out var days) || days < 1 || days > 366)
                        {
                            await bot.SendMessage(chatId, "Введи ціле число від 1 до 366:", replyMarkup: Menus.SettingsReply(), cancellationToken: ct);
                            return;
                        }

                        s.Days = days;
                        s.Step = PendingInput.Review;

                        await bot.SendMessage(chatId, BuildSummary(s), replyMarkup: Menus.Review(), cancellationToken: ct);
                        return;
                    }

                case PendingInput.ChooseCard:
                    await bot.SendMessage(chatId, "Обери картку кнопкою під повідомленням 👇", cancellationToken: ct);
                    return;

                case PendingInput.Review:
                    await bot.SendMessage(chatId, "Натисни Підтвердити або Заповнити заново під повідомленням.", cancellationToken: ct);
                    return;

                default:
                    await bot.SendMessage(chatId, "Обери дію в меню 👇", replyMarkup: Menus.MainReply(), cancellationToken: ct);
                    return;
            }
        }

        private static string BuildSummary(UserSession s)
        {
            var selectedPan = "-";
            var acc = s.Accounts?.FirstOrDefault(a => a.id == s.SelectedAccountId);
            if (acc != null) selectedPan = acc.maskedPan?.FirstOrDefault() ?? "-";

            return
                "✅ Дані зібрано.\n\n" +
                $"Картка: {selectedPan}\n" +
                $"Ціль: {s.Goal}\n" +
                $"Днів до зарплати: {s.Days}\n\n" +
                "Підтвердити налаштування?";
        }
    }
}
