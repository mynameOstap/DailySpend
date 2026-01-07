namespace DailySpendServer.Model
{
    public class UserSetting
    {
        public string id { get; set; } = string.Empty;
        public string ChatId { get; set; } = string.Empty;  
        public string BankAccountId { get; set; } = string.Empty;
        public BankAccount? BankAccount { get; set; } = null;
        public string DailyPlanId { get; set; } = string.Empty;
        public ICollection<DailyPlan> DailyPlans { get; set; } = new List<DailyPlan>();
        public string NotificationId { get; set; } = string.Empty;
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public string name { get; set; } = string.Empty;
        public int GoalAmount { get; set; }
        public string token { get; set; } = string.Empty;
        public int daysToSalary { get; set; }
        public string SelectedAccountId { get; set; } = string.Empty;
        public string WebHookSecret { get; set; } = string.Empty;
    }
}
