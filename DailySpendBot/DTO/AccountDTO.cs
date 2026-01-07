using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailySpendBot.DTO
{
  

    public class AccountDTO
    {
        public string id { get; set; } = "";
        public List<string> maskedPan { get; set; } = new(); 
        public int Balance { get; set; }
      
    }
}
