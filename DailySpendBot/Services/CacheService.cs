using DailySpendBot.Messages;
using DailySpendServer.DTO;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailySpendBot.Services
{
    public class CacheService
    {
        private readonly IMemoryCache _cache;
        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GetUserStatus(string userId)
        {
            var exist =  _cache.TryGetValue(userId, out string? status);
            if(exist && !string.IsNullOrEmpty(status))
                return status;
            return null!;
        }
        public void SetUserStatus(string userId, string status)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(1));
            _cache.Set(userId, status, cacheEntryOptions);
        }
    }
}
