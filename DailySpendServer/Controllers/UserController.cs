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
        private readonly WebHookService _hookService;
        public UserController(DailySpendContext db, HttpSender httpSender,CalculateOutley calculateOutley,WebHookService hookService)
        {
            _db = db;
            _httpSender = httpSender;
            _calculateOutley = calculateOutley;
            _hookService = hookService;
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
        [HttpPost("api/users/createorupdate")]
        public async Task<IActionResult> CreateOrUpdateUser([FromBody] UserSettingDTO dto)
        {
            _httpSender.AddToken(dto.token);

            var user = await _db.UserSettings
                .Include(u => u.BankAccount)
                .FirstOrDefaultAsync(u => u.id == dto.id);

            if (user != null)
            {
                
                user.ChatId = dto.ChatId;
                user.name = dto.name;
                user.GoalAmount = dto.GoalAmount;
                user.token = dto.token;
                user.daysToSalary = dto.daysToSalary;
                user.SelectedAccountId = dto.SelectedAccountId;
                user.IsActive = true; 

                if (user.BankAccount != null)
                {
                    if (user.BankAccount.id != dto.SelectedAccountId)
                    {
                        _db.BankAccounts.Remove(user.BankAccount);
                        _db.BankAccounts.Add(CreateBankAccount(dto));
                    }
                    else
                    {
                        user.BankAccount.MaskedPan = dto.maskedPan;
                        user.BankAccount.Balance = dto.balance ?? 0;
                        user.BankAccount.Name = dto.name;
                        user.BankAccount.WebHookUrl = null; 
                    }
                }
                else
                {
                    _db.BankAccounts.Add(CreateBankAccount(dto));
                }

                _db.UserSettings.Update(user);
            }
            else
            {

                var newUser = new UserSetting
                {
                    id = dto.id,
                    ChatId = dto.ChatId,
                    name = dto.name,
                    GoalAmount = dto.GoalAmount,
                    token = dto.token,
                    daysToSalary = dto.daysToSalary,
                    SelectedAccountId = dto.SelectedAccountId,
                    IsActive = true 
                };

                _db.UserSettings.Add(newUser);
                _db.BankAccounts.Add(CreateBankAccount(dto));
            }

            await _db.SaveChangesAsync();

            try 
            {
                await _hookService.RegisterWebHook(dto.id);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Webhook error: {ex.Message}");
            }

            return Ok(dto);
        }

       
        private BankAccount CreateBankAccount(UserSettingDTO dto)
        {
            return new BankAccount
            {
                id = dto.SelectedAccountId,
                UserSettingId = dto.id,
                MaskedPan = dto.maskedPan,
                Balance = dto.balance ?? 0,
                Name = dto.name
            };
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
        [HttpPost("api/user/notification/mark/{notificationId}")]
        public async Task<IActionResult> MarkNotificationAsSent([FromRoute] string eventMessageId)
        {
            var eventMessage = await _db.UserEventMessages.FirstOrDefaultAsync(n => n.Id == eventMessageId);
            if (eventMessage == null)
            {
                return NotFound();
            }
            if (eventMessage.IsSent) return Ok();

            eventMessage.IsSent = true;
            await _db.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("api/user/notification")]
        public async Task<IActionResult> GetPendingNotifications()
        {
            var eventMessages = await _db.UserEventMessages
                .AsNoTracking()
                .Where(m => !m.IsSent)
                .Take(20)
                .ToListAsync();
          
            return Ok(eventMessages);
        }

    }
}
