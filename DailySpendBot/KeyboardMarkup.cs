using DailySpendBot.DTO;
using Telegram.Bot.Types.ReplyMarkups;

public static class Menus
{
    public const string BtnStatus = "Статус";
    public const string BtnSettings = "Налаштування";

    public const string BtnSetToken = "Змінити токен";
    public const string BtnSetGoal = "Змінити ціль";
    public const string BtnSetDays = "Змінити дні до зарплати";
    public const string BtnSetCard = "Змінити карту";
    public const string BtnBackToMain = "⬅️ Меню";

    public static ReplyKeyboardMarkup MainReply() => new(new[]
    {
        new KeyboardButton[] { new(BtnStatus), new(BtnSettings) }
    })
    {
        ResizeKeyboard = true,
        OneTimeKeyboard = true
    };

    public static ReplyKeyboardMarkup SettingsReply() => new(new[]
    {
        new KeyboardButton[] { new(BtnSetToken), new(BtnSetGoal) },
        new KeyboardButton[] { new(BtnSetDays),  new(BtnSetCard) },
        new KeyboardButton[] { new(BtnBackToMain) }
    })
    {
        ResizeKeyboard = true,
        OneTimeKeyboard = true
    };

    public static InlineKeyboardMarkup CardsMenu(IReadOnlyList<AccountDTO> accounts)
    {
        var rows = accounts.Select(a =>
        {
            var panText = a.maskedPan?.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(panText))
                panText = "";

            return new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    text: panText!,
                    callbackData: $"acc:{a.id}"
                )
            };
        }).ToList();

        rows.Add(new[]
        {
            InlineKeyboardButton.WithCallbackData("Інший токен", "token:retry"),
            InlineKeyboardButton.WithCallbackData("Меню", "menu:main")
        });

        return new InlineKeyboardMarkup(rows);
    }

    public static InlineKeyboardMarkup Review() => new(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Підтвердити", "confirm"),
            InlineKeyboardButton.WithCallbackData("Заповнити заново", "cancel")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Меню", "menu:main")
        }
    });

    public static InlineKeyboardMarkup TokenRetry() => new(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Інший токен", "token:retry"),
            InlineKeyboardButton.WithCallbackData("Меню", "menu:main")

       }
     });
  }
