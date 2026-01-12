using DailySpendServer.Model;

namespace DailySpendServer.DTO
{
    public class StatusResponseDTO
    {
        public string UserSettingId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int PlannedAmount { get; set; }
        public long SpentAmount { get; set; }
        public long Balance { get; set; }
        public int daysToSalary { get; set; }

    }
}
