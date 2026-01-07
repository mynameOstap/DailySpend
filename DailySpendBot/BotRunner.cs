using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class BotRunner : BackgroundService
{
    private readonly ITelegramBotClient _bot;
    private readonly UpdateRouter _router;
    private readonly ILogger<BotRunner> _logger;

    public BotRunner(ITelegramBotClient bot, UpdateRouter router, ILogger<BotRunner> logger)
    {
        _bot = bot;
        _router = router;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _bot.StartReceiving(
            updateHandler: (client, update, ct) => _router.Handle(client, update, ct),
            errorHandler: (client, ex, source, ct) =>
            {
                _logger.LogError(ex, "Polling error (source: {Source})", source);
                return Task.CompletedTask;
            },
            receiverOptions: new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            },
            cancellationToken: stoppingToken
        );

        _logger.LogInformation("Bot is running.");
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
