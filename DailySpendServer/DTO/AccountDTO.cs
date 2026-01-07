namespace DailySpendServer.DTO
{
    public class AccountDTO
    {
        public string id { get; set; } = string.Empty;
        public int balance { get; set; }
        public List<string> maskedPan { get; set; } = new();
       
    }
}
