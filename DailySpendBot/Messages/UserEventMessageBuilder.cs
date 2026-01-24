using System.Text;
using DailySpendBot.DTO;

namespace DailySpendBot.Messages;

public static class UserEventMessageBuilder
{
    public static string Build(UserEventMessageDTO message)
    {
        return new StringBuilder()
            .AppendLine("<b>Notification:</b>")
            .AppendLine(message.Message)
            .ToString();
    }
    
}