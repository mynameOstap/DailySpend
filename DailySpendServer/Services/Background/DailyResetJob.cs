namespace DailySpendServer.Services.Background
{
    public class DailyResetJob : BackgroundService
    {
        private readonly DailyResetService _dailyResetService;
        private DateTime _lastRunDate = DateTime.UtcNow.Date;

        public DailyResetJob(DailyResetService dailyResetService)
        {
            _dailyResetService = dailyResetService;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var today = DateTime.UtcNow.Date;
                    if (today > _lastRunDate)
                    {
                        await _dailyResetService.ResetDailyPlansAsync(today);
                        _lastRunDate = today;
                    } 

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in DailyResetJob: {ex.Message}");
                }
                await Task.Delay(TimeSpan.FromHours(1), ct);
            }
        }
    }
}
