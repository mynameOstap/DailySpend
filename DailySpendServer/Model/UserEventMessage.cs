namespace DailySpendServer.Model;

public class UserEventMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string UserId { get; set; } = default!;
    public string Type { get; set; } = default!; 
    public string Message { get; set; } = default!;
    public bool IsSent { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
