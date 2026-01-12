using DailySpendServer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailySpendBot.Messages
{
    public static class StatusMessageBuilder
    {
        public static string Build(StatusResponseDTO status)
        {
            var remaining = status.PlannedAmount - status.SpentAmount;

            var percent = status.PlannedAmount > 0
                ? (int)((double)status.SpentAmount / status.PlannedAmount * 100)
                : 0;

            var emoji = remaining >= 0 ? "🟢" : "🔴";

            return
                $@"📊 *Фінансовий статус*

                📅 {status.Date:dd.MM.yyyy}

                💰 Баланс: *{status.Balance:N0} ₴*
                🎯 Ліміт: *{status.PlannedAmount:N0} ₴*
                💸 Витрачено: *{status.SpentAmount:N0} ₴*


                ⏳ Днів до зарплати: *{status.daysToSalary}*

                {emoji} Залишок: *{remaining:N0} ₴*";
        }

        
    }
}
