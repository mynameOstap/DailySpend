
using DailySpendServer.Data;
using DailySpendServer.DTO;
using DailySpendServer.Services;
using DailySpendServer.Services.Background;
using DailySpendServer.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();


builder.Services.AddDbContext<DailySpendContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DailySpendDatabase"))
);

var baseUrl = builder.Configuration.GetValue<string>("Url")
             ?? throw new InvalidOperationException("Config 'Url' is missing.");
builder.Services.AddHttpClient<HttpSender>(HttpSender =>
{
    HttpSender.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddOptions<MonobankOptions>()
    .Bind(configuration.GetSection("MonobankOptions"))
    .Validate(o => !string.IsNullOrWhiteSpace(o.BaseUrl), "Monobank:BaseUrl is required")
    .ValidateOnStart();

builder.Services.AddControllers();
builder.Services.AddScoped<WebHookService>();
builder.Services.AddScoped<GoalTrackingService>();
builder.Services.AddScoped<DailyResetService>();
builder.Services.AddScoped<CalculateOutley>();

builder.Services.AddHostedService<DailyResetJob>();
builder.Services.AddHostedService<WebHookRecoveryJob>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DailySpendContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
