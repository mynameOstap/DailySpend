using DailySpendServer.DTO;
using DailySpendServer.Shared;
using Microsoft.Extensions.Options;

namespace DailySpendServer.Services
{
    public class WebHookService
    {
        private readonly HttpSender _httpSender;
        private readonly MonobankOptions _monobankOptions;
        public WebHookService(HttpSender httpSender, IOptions<MonobankOptions> options)
        {
            _httpSender = httpSender;
            _monobankOptions = options.Value;
        }
        
        public async Task RegisterWebHook(string userId, string s)
        {
            var url = $"{_monobankOptions.BaseUrl}/api/monobank/webhook/{userId}?s={s}";
            var response = await _httpSender.PostJsonAsync("https://api.monobank.ua/personal/webhook", new { webHookUrl = url });
            var responseString = await response.Content.ReadAsStringAsync();
            if(!response.IsSuccessStatusCode)
                throw new Exception($"Failed to register webhook: HTTP {(int)response.StatusCode}: {responseString}");
        }
    }
}
