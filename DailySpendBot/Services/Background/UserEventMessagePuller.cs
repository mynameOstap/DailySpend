using Microsoft.Extensions.Hosting;

namespace DailySpendBot.Services.Background;

public class UserEventMessagePuller : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IServiceScopeFactory _scopeFactory;

    public UserEventMessagePuller()
    {
        
    }
    
}