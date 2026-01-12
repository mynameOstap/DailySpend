using DailySpendServer.Data;
using Microsoft.EntityFrameworkCore;

namespace DailySpendServer.Services
{
    public class DailyResetService
    {
        private readonly DailySpendContext _db;
        private readonly CalculateOutley _calculateOutley;
        public DailyResetService(DailySpendContext db, CalculateOutley calculateOutley)
        {
            _db = db;
            _calculateOutley = calculateOutley;
        }

        public async Task ResetDailyPlansAsync(DateTime date)
        {
            var users = await _db.UserSettings.Include(u => u.BankAccount).ToListAsync();

            foreach (var user in users)
            {
                if (user.BankAccount == null)
                    continue;
                var existingPlan = await _db.DailyPlans
                    .FirstOrDefaultAsync(p => p.UserSettingId == user.id && p.Date == date);
                if (existingPlan != null)
                    continue;
                if (existingPlan == null)
                {
                    await _calculateOutley.CreateDailyPlan(user.id, date);
                }
               
            }
        }
    }
}
