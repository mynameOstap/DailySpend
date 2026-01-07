using DailySpendBot.Handlers;
using Telegram.Bot;
using Telegram.Bot.Types;

public class UpdateRouter
{
    private readonly MessageHandler _msg;
    private readonly CallBackHandler _cb;

    public UpdateRouter(MessageHandler msg, CallBackHandler cb)
    {
        _msg = msg;
        _cb = cb;
    }

    public async Task Handle(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        if (update.CallbackQuery is { } cq)
            await _cb.Handle(bot, cq, ct);
        else if (update.Message is { } msg)
            await _msg.HandleMessage(bot, msg, ct);
    }

}
