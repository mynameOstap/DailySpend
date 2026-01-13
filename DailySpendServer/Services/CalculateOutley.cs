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

            var date = update.Time.Value.ToUniversalTime().Date;

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
            var user = await _db.UserSettings.Where(u => u.IsActive)
                .Include(u => u.BankAccount)
                .FirstOrDefaultAsync(u => u.id == userId);

            if (user == null)
                return null;
            
            if (user?.BankAccount == null)
                return null;

            var plan = new DailyPlan
            {
                UserSettingId = userId,
                Date = date,
                PlannedAmount = CalculateDailyLimit(user.BankAccount.Balance, user.GoalAmount, user.daysToSalary),
                SpentAmount = 0
            };

            var previousPlan = await _db.DailyPlans
                .Where(p => p.UserSettingId == user.id)
                .OrderByDescending(p => p.Date)
                .FirstOrDefaultAsync();
            var yesterday = date.AddDays(-1);
            if (previousPlan != null && previousPlan.Date == yesterday)
            {
                if (previousPlan.PlannedAmount >= previousPlan.SpentAmount)
                {
                    previousPlan.Completed = true;

                }
                else
                { 
                    previousPlan.Completed = false;
                }

                user.daysToSalary = Math.Max(0, user.daysToSalary - 1);
            }

            _db.DailyPlans.Add(plan);
            await _db.SaveChangesAsync();

            return await _db.DailyPlans
                .Include(p => p.UserSetting)
                    .ThenInclude(u => u.BankAccount)
                .FirstAsync(p => p.id == plan.id);
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
