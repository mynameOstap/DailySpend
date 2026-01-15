using DailySpendBot.Handlers;
using DailySpendBot.Services;
using DailySpendBot.Services.Background;
using DailySpendBot.Sessions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        var botToken = ctx.Configuration["Telegram:Token"]
            ?? throw new InvalidOperationException("Telegram:Token is missing");

        services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(botToken));

        services.AddSingleton<BotSessionStore>();
        services.AddHttpClient<BotHttpClient>(client =>
        {
            client.BaseAddress = new Uri(ctx.Configuration["Backend:BaseUrl"]!);
        });
        services.AddHostedService<NotificationPuller>();

        services.AddSingleton<MessageHandler>();
        services.AddSingleton<CallBackHandler>();
        services.AddSingleton<UpdateRouter>(); 

        services.AddHostedService<BotRunner>(); 
    });

await builder.RunConsoleAsync();
