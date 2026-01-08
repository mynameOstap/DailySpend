namespace DailySpendServer.Model
{
    public class DailyPlan
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string UserSettingId { get; set; } = string.Empty;
        public UserSetting? UserSetting { get; set; }
        public DateTime Date { get; set; }
        public int PlannedAmount { get; set; }
        public long SpentAmount { get; set; }
    }
}
