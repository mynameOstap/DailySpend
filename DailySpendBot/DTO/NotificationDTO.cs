using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailySpendBot.DTO
{
    public class NotificationDTO
    {
        public string id { get; set; }  =  string.Empty;
        public string UserSettingId { get; set; }  =  string.Empty;
        public string ChatId { get; set; }  =  string.Empty;
        public DateTime Time { get; set; }
        public string? Description { get; set; }
        public long Amount { get; set; }
        public long Balance { get; set; }
    }
}
