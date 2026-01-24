using DailySpendServer.Data;
using DailySpendServer.Enums;
using DailySpendServer.Model;
using Microsoft.EntityFrameworkCore;

namespace DailySpendServer.Services;

public class GoalTrackingService
{
    private readonly DailySpendContext _db;

    public GoalTrackingService(DailySpendContext db)
    {
        _db = db;
    }
    public async Task<GoalResult> CheckGoal(string userId)
    {
        var user = await _db.UserSettings
            .Include(u => u.BankAccount)
            .FirstOrDefaultAsync(u => u.id == userId);

        if (user?.BankAccount == null)
            return GoalResult.None;

        var balance = user.BankAccount.Balance / 100;

        if (balance < user.GoalAmount)
        {
            user.IsActive = false;
            user.BankAccount.WebHookUrl = null;
            _db.UserEvents.Add(new UserEventMessage()
            {
                UserId = user.id,
                ChatId = user.ChatId,
                Type = UserEventType.ResetSetting,
                Message = "Нажаль вам не вдалось заощати гроші, щоб задати нову ціль натискайте налаштування",
                CreatedAt = DateTime.UtcNow,
                IsSent = false

            });
            await _db.SaveChangesAsync();
            return GoalResult.Failed;
        }

        if (user.daysToSalary <= 0 && balance >= user.GoalAmount)
        {
            user.IsActive = false;
            user.BankAccount.WebHookUrl = null;
            _db.UserEvents.Add(new UserEventMessage()
            {
                UserId = user.id,
                ChatId = user.ChatId,
                Type = UserEventType.ResetSetting,
                Message = "Вітаю ви справжній молодець ,щоб задати нову ціль натискайте налаштування",
                CreatedAt = DateTime.UtcNow,
                IsSent = false

            });
            await _db.SaveChangesAsync();
            return GoalResult.Completed;
        }

        return GoalResult.None;
    }

}   