using DailySpendServer.Data;
using DailySpendServer.DTO;
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

        public async Task CalculateDailyPlan(string userId, NotificationUpdateDTO? update = null)
        {
            var user = await _db.UserSettings.AsNoTracking().FirstOrDefaultAsync(x => x.id == userId);
            if (user == null)
                return;

            var existingPlans = await _db.DailyPlans.Where(x => x.UserSettingId == userId && x.Date == DateTime.Today).FirstOrDefaultAsync();
            if (existingPlans == null)
            { 
                
            }

        }

    }
}
