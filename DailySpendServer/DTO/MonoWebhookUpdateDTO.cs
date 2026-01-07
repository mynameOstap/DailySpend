public sealed class MonoWebhookUpdateDTO
{
    public string? Type { get; set; } 
    public MonoWebhookDataDTO? Data { get; set; }
}

public sealed class MonoWebhookDataDTO
{
    public string? Account { get; set; }
    public MonoStatementItemDTO? StatementItem { get; set; }
}

public sealed class MonoStatementItemDTO
{
    public string? Id { get; set; }
    public long Time { get; set; }
    public string? Description { get; set; }
    public bool Hold { get; set; }
    public long Amount { get; set; }
    public long Balance { get; set; }
    public int CurrencyCode { get; set; }
}
