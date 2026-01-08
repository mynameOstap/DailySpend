using DailySpendServer.Data;
using DailySpendServer.DTO;
using DailySpendServer.Model;
using Microsoft.EntityFrameworkCore;

namespace DailySpendServer.Services
{
    public class CalculateOutley
    {
        private readonly DailySpendContext _db;
        public CalculateOutley(DailySpendContext db)
        {
            _db = db;
        }

        public async Task ApplyExpense( NotificationUpdateDTO update)
        {
            if (update.Amount == null || update.Amount >= 0)
                return; 

            if (update.Time == null)
                return;

            var date = update.Time.Value.Date;

            var plan = await _db.DailyPlans
                .FirstOrDefaultAsync(p =>
                    p.UserSettingId == update.UserSettingId &&
                    p.Date == date);

            if (plan == null)
            {
                plan = await CreateDailyPlan(update.UserSettingId!, date);
                if (plan == null)
                    return;
            }

            plan.SpentAmount += Math.Abs(update.Amount.Value); 
            await _db.SaveChangesAsync();
        }

        public async Task<DailyPlan?> CreateDailyPlan(string userId, DateTime date)
        {
            var user = await _db.UserSettings
                .Include(u => u.BankAccount)
                .FirstOrDefaultAsync(u => u.id == userId);

            if (user?.BankAccount == null)
                return null;

            var plan = new DailyPlan
            {
                UserSettingId = userId,
                Date = date,
                PlannedAmount = CalculateDailyLimit(user.BankAccount.Balance, user.GoalAmount, user.daysToSalary),
                SpentAmount = 0
            };

            _db.DailyPlans.Add(plan);
            await _db.SaveChangesAsync();

            return plan;
        }

        private int CalculateDailyLimit(long balance, int goal, int daysLeft)
        {
            if (daysLeft <= 0)
                return 0;

            var balanceUah = balance / 100; 

            var available = balanceUah - goal;
            if (available <= 0)
                return 0;

            return (int)available / daysLeft; 
        }

    }
}
