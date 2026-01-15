using DailySpendServer.Data;
using DailySpendServer.DTO;
using DailySpendServer.Model;
using DailySpendServer.Services;
using DailySpendServer.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DailySpendServer.Controllers
{
    [ApiController]
    public class UserController : Controller
    {
        private readonly DailySpendContext _db;
        private readonly HttpSender _httpSender;
        private readonly CalculateOutley _calculateOutley;
        public UserController(DailySpendContext db, HttpSender httpSender,CalculateOutley calculateOutley)
        {
            _db = db;
            _httpSender = httpSender;
            _calculateOutley = calculateOutley; 
        }

        [HttpGet("api/users/settings")]
        public async Task<IActionResult> GetUserSetting([FromQuery] string userId)
        {
            var userExist = await _db.UserSettings.FirstOrDefaultAsync(u => u.id == userId);
            if (userExist == null)
            {
                return NotFound();
            }

            return Ok(userExist);
        }
        [HttpPost("api/users/create")]
        public async Task<IActionResult> CreateUserSetting([FromBody] UserSettingDTO userSetting)
        {
            var userExist = await _db.UserSettings.FirstOrDefaultAsync(u => u.id == userSetting.id);
            if (userExist != null)
            {
                return Conflict();
            }
            _httpSender.AddToken(userSetting.token);

            var newUserSetting = new UserSetting
            {
                id = userSetting.id,
                ChatId = userSetting.ChatId,
                name = userSetting.name,
                GoalAmount = userSetting.GoalAmount,
                token = userSetting.token,
                daysToSalary = userSetting.daysToSalary,
                SelectedAccountId = userSetting.SelectedAccountId
            };
            var newBankAccount = new BankAccount()
            {
                id = userSetting.SelectedAccountId,
                UserSettingId = userSetting.id,
                MaskedPan = userSetting.maskedPan,
                Balance = userSetting.balance ?? 0,
                Name = userSetting.name
            };
            _db.UserSettings.Add(newUserSetting);
            _db.BankAccounts.Add(newBankAccount);
            
            await _db.SaveChangesAsync();
            return Ok(userSetting);
        }
        [HttpGet("api/users/status")]
        public async Task<IActionResult> GetStatus([FromQuery] string userId)
        {
            var today = DateTime.UtcNow.Date;

            var plan = await _db.DailyPlans
                .Include(p => p.UserSetting)
                    .ThenInclude(u => u.BankAccount)
                .FirstOrDefaultAsync(p =>
                    p.UserSettingId == userId &&
                    p.Date == today);

            if (plan == null)
            {
                plan = await _calculateOutley.CreateDailyPlan(userId, today);
                if (plan == null)
                    return NotFound();
            }

            return Ok(new StatusResponseDTO
            {
                UserSettingId = plan.UserSettingId,
                PlannedAmount = plan.PlannedAmount,
                SpentAmount = plan.SpentAmount,
                Balance = plan.UserSetting?.BankAccount?.Balance ?? 0,
                daysToSalary = plan.UserSetting?.daysToSalary ?? 0
            });
        }

    }
}
