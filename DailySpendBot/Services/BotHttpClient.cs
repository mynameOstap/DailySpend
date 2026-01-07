using DailySpendBot.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DailySpendBot.Services
{
    public class BotHttpClient
    {
        private readonly HttpClient _httpClient;
        public BotHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<AccountDTO>?> ValidateToken(string token)
        {
            var response = await _httpClient.PostAsync(
                "api/bank/validateToken",
                new StringContent(JsonSerializer.Serialize(token), Encoding.UTF8, "application/json")
            );

            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"ValidateToken failed: {(int)response.StatusCode} body='{body}'");
                return null;
            }

            if (string.IsNullOrWhiteSpace(body))
                return null;

            return JsonSerializer.Deserialize<List<AccountDTO>>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        public async Task<string> GetUserStatus(string userId)
        {
            var response = await _httpClient.GetAsync($"api/users/status?userId={Uri.UnescapeDataString(userId)}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return content;
        }
        public async Task<UserSettingDTO> GetUserSettings(string userId)
        {
            var response = await _httpClient.GetAsync($"api/users/settings?userId={Uri.UnescapeDataString(userId)}");
            var strContent = await response.Content.ReadAsStringAsync();
            var content = JsonSerializer.Deserialize<UserSettingDTO>(strContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return content;
        }
        
        public async Task<string> CreateUser(UserSettingDTO userSetting)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(userSetting), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/users/create", jsonContent);
            var content = await response.Content.ReadAsStringAsync();
            return content;

        }
        public async Task<T>GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content) ?? throw new Exception($"Deserialize<{typeof(T).Name}> returned null. Body: {content}");
        }

        public async Task<bool> MarkNotification(string url)
        {
            var response = await _httpClient.PostAsync(url, null);
            return response.IsSuccessStatusCode;
        }
    }
}
