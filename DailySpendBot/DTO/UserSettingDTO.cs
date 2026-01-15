using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailySpendBot.DTO
{
    public class UserSettingDTO
    {
        public string id { get; set; } = string.Empty;
        public string ChatId { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public int GoalAmount { get; set; }
        public string token { get; set; } = string.Empty;
        public int daysToSalary { get; set; }
        public string SelectedAccountId { get; set; } = string.Empty;
        public string maskedPan { get; set; } = string.Empty;
        public long? balance { get; set; } 
    }
}
