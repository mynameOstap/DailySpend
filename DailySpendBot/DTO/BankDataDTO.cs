namespace DailySpendBot.DTO;

public class BankDataDTO
{
    public string id { get; set; } = string.Empty;
    public string maskedPan { get; set; } = string.Empty;
    public long? balance { get; set; } 
}