namespace DailySpendServer.Model
{
    public class BankAccount
    {
        public string id { get; set; } = string.Empty;
        public string UserSettingId { get; set; } = string.Empty;
        public UserSetting? UserSetting { get; set; }
        public string Name { get; set; } = string.Empty;
        public string WebHookUrl { get; set; } = string.Empty;
        public string CashbackType { get; set; } = string.Empty;
        public int Balance { get; set; }
        public string cardId { get; set; } = string.Empty;

        public string MaskedPan { get; set; } = string.Empty;
    }
}
