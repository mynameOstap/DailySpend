using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailySpendBot.Sessions
{
    public class BotSessionStore
    {
        private readonly ConcurrentDictionary<long, UserSession> _sessions = new();

        public UserSession GetOrCreate(long userId)
            => _sessions.GetOrAdd(userId, _ => new UserSession());

        public bool TryGet(long userId, out UserSession session)
            => _sessions.TryGetValue(userId, out session!);

        public void Clear(long userId)
            => _sessions.TryRemove(userId, out _);
    }
}
