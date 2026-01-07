

using DailySpendServer.DTO;
using DailySpendServer.Model;
using System.Text.Json;

namespace DailySpendServer.Shared
{
    public class HttpSender
    {
        private readonly HttpClient _httpClient;

        public HttpSender(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> GetTAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content) ?? throw new Exception("Deserialization returned null");
        }
        public async Task<string> GetStringAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<HttpResponseMessage> PostJsonAsync<T>(string url, T data)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = JsonContent.Create(data);
            var response = await _httpClient.SendAsync(request);
            return response;

        }
        public async Task<string> PostJsonAsync<T>(string url)
        {
            var response = await _httpClient.PostAsync(url, null);
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<T> GetTAsync<T>(string url, string token)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Add("X-Token", token);

            using var response = await _httpClient.SendAsync(req);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"HTTP {(int)response.StatusCode}: {content}");


            return JsonSerializer.Deserialize<T>(content)
                   ?? throw new Exception("Deserialization returned null");
        }
        public async Task<BankAccountDTO> ValidateToken(string url, string token)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Add("X-Token", token);

            using var response = await _httpClient.SendAsync(req);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"HTTP {(int)response.StatusCode}: {content}");

            var bankAccount = JsonSerializer.Deserialize<BankAccountDTO>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return bankAccount ?? throw new Exception("Monobank response deserialize returned null");
        }
        public void AddToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Remove("X-Token");
            _httpClient.DefaultRequestHeaders.Add("X-Token", token);
        }
    }
}
