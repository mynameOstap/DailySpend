using DailySpendServer.Data;
using DailySpendServer.DTO;
using DailySpendServer.Model;
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
        public UserController(DailySpendContext db, HttpSender httpSender)
        {
            _db = db;
            _httpSender = httpSender;
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
            _db.UserSettings.Add(newUserSetting);

            await _db.SaveChangesAsync();
            return Ok(userSetting);
        }
        [HttpGet("api/users/status")]
        public async Task<IActionResult> GetStatus([FromQuery] string userId)
        {
           var status = await _db.DailyPlans.FirstOrDefaultAsync(s => s.UserSetting.id == userId);
              if (status == null)
              {
                return NotFound();
            }
              return Ok(status);
        }
    }
}
