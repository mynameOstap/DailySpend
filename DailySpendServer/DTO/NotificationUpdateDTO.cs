namespace DailySpendServer.DTO
{
    public class NotificationUpdateDTO
    {
        public string? UserSettingId { get; set; } = string.Empty;
        public DateTime? Time { get; set; }
        public long? Amount { get; set; }
        public long? Balance { get; set; }
    }
}
