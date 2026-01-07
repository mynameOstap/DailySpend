namespace DailySpendServer.Model
{
    public class Notification
    {
        public string id { get; set; } = Guid.NewGuid().ToString("N");
        public string UserSettingId { get; set; } = string.Empty;
        public string ChatId { get; set; } = string.Empty;
        public UserSetting? UserSetting { get; set; }
        public DateTime Time { get; set; }
        public string? Description { get; set; }
        public long Amount { get; set; }
        public long Balance { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsSent { get; set; } = false;
    }
}
