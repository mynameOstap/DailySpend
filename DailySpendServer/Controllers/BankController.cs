using DailySpendServer.Data;
using DailySpendServer.DTO;
using DailySpendServer.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using DailySpendServer.Model;

namespace DailySpendServer.Controllers
{
    [ApiController]
    public class BankController : Controller
    {
        private readonly HttpSender _httpSender;
        private readonly DailySpendContext _db;

        public BankController(HttpSender httpSender, DailySpendContext db)
        {
            _httpSender = httpSender;
            _db = db;
        }

        [HttpPost("api/bank/validateToken")]
        public async Task<IActionResult> ValidateToken([FromBody] string token)
        {
            var bankAccount = await _httpSender.ValidateToken("https://api.monobank.ua/personal/client-info", token);
            var accounts = bankAccount.accounts.Select(a => new AccountDTO
            {
                id = a.id,
                balance = a.balance,
                maskedPan = a.maskedPan
            }).ToList();
            if (accounts.Count == 0)
            {
                return Unauthorized();
            }
            return Ok(accounts);

        }

        [HttpGet("api/monobank/webhook/{userId}")]
        public async Task<IActionResult> ValidateWebHook([FromRoute] string userId, [FromQuery] string s)
        {
            var user = await _db.UserSettings.AsNoTracking().FirstOrDefaultAsync(u => u.id == userId);
            if (user == null || user.WebHookSecret != s || string.IsNullOrWhiteSpace(user.WebHookSecret))
            {
                return Unauthorized();
            }
            return Ok();
        }

        [HttpPost("api/monobank/webhook/{userId}")]
        public async Task<IActionResult> ReceiveWebHook([FromRoute] string userId, [FromQuery] string s, [FromBody] MonoWebhookUpdateDTO update)
        {
            var user = await _db.UserSettings.AsNoTracking().FirstOrDefaultAsync(u => u.id == userId);
            if (user == null || user.WebHookSecret != s || string.IsNullOrWhiteSpace(user.WebHookSecret))
            {
                return Unauthorized();
            }
            var item = update?.Data?.StatementItem;
            if (item == null)
            {
                return Ok();
            }
            _db.Notifications.Add(new Notification
            {

                UserSettingId = userId,
                ChatId = user.ChatId,
                Amount = item.Amount,
                Description = item.Description,
                Time = DateTimeOffset.FromUnixTimeSeconds(item.Time).DateTime,
                Balance = item.Balance

            });

            return Ok();
        }

        [HttpPost("api/monobank/notification/mark/{notificationId}")]
        public async Task<IActionResult> MarkNotificationAsSent([FromRoute] string notificationId)
        {
            var notification = await _db.Notifications.FirstOrDefaultAsync(n => n.id == notificationId);
            if (notification == null)
            {
                return NotFound();
            }
            if (notification.IsSent) return Ok();

            notification.IsSent = true;
            notification.SentAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("api/monobank/notification")]
        public async Task<IActionResult> GetPendingNotifications()
        {
            var notifications = await _db.Notifications
                .AsNoTracking()
                .Where(n => !n.IsSent)
                .Take(20)
                .ToListAsync();
            var notificationDTOs = notifications.Select(n => new Notification
            {
                id = n.id,
                UserSettingId = n.UserSettingId,
                ChatId = n.ChatId,
                Time = n.Time,
                Description = n.Description,
                Amount = n.Amount,
                Balance = n.Balance
            }).ToList();
            return Ok(notificationDTOs);
        }
    }
}
