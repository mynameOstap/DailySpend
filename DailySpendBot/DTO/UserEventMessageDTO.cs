using DailySpendBot.Enums;

namespace DailySpendBot.DTO;


    public class UserEventMessageDTO
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = default!;
        public string ChatId { get; set; } = default!;
        public UserEventType Type { get; set; } = UserEventType.None; 
        public string Message { get; set; } = default!;
        public bool IsSent { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

