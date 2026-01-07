namespace DailySpendServer.DTO
{
    public class BankAccountDTO
    {
        public string clientId { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string webHookUrl { get; set; } = string.Empty;

        public List<AccountDTO> accounts { get; set; } = new();
    }
   
}
