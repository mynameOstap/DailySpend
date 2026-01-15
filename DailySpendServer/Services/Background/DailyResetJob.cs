namespace DailySpendServer.Services.Background
{
    public class DailyResetJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private DateTime _lastRunDate = DateTime.UtcNow.Date;

        public DailyResetJob(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var _dailyResetService = scope.ServiceProvider.GetRequiredService<DailyResetService>();
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
