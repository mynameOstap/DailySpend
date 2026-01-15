using DailySpendServer.Data;
using DailySpendServer.DTO;
using DailySpendServer.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace DailySpendServer.Services
{
    public class WebHookService
    {
        private readonly HttpSender _httpSender;
        private readonly MonobankOptions _monobankOptions;
        private readonly DailySpendContext _db;
        public WebHookService(HttpSender httpSender, IOptions<MonobankOptions> options, DailySpendContext db)
        {
            _httpSender = httpSender;
            _monobankOptions = options.Value;
            _db = db;
        }
        
        public async Task RegisterWebHook(string userId)
        {
            var user = await _db.UserSettings
                .Include(b => b.BankAccount)
                .FirstOrDefaultAsync(u => u.id == userId);

            if (user == null || user.BankAccount == null)
                throw new Exception("User not found");

            if (string.IsNullOrEmpty(user.WebHookSecret))
                user.WebHookSecret = Guid.NewGuid().ToString("N");

            var url = $"{_monobankOptions.BaseUrl}/api/monobank/webhook/{userId}?s={user.WebHookSecret}";

            var response = await _httpSender.PostJsonAsync(
                "https://api.monobank.ua/personal/webhook",
                new { webHookUrl = url });

            if (!response.IsSuccessStatusCode)
                throw new Exception("Webhook registration failed");

            user.BankAccount.WebHookUrl = url;
            user.WebHookActive = true;
            await _db.SaveChangesAsync();
        }
    }
}
