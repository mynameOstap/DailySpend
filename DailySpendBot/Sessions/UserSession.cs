using DailySpendBot.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailySpendBot.Sessions
{
    public class UserSession
    {
        public PendingInput Step { get; set; } = PendingInput.None;

        public string? id { get; set; }
        public string? Token { get; set; }
        public int? Goal { get; set; }
        public int? Days { get; set; }
        public List<AccountDTO> Accounts { get; set; } = new();
        public string? SelectedAccountId { get; set; }
        public string maskedPan { get; set; } = string.Empty;
        public long balance { get; set; } 
    }
}
