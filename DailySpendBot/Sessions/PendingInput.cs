using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailySpendBot.Sessions
{
    public enum PendingInput
    {
        None = 0,
        Token = 1,
        ChooseCard = 2,
        Goal = 3,
        Days = 4,
        Review = 5
    }
}
