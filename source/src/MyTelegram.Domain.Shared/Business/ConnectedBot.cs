namespace MyTelegram.Domain.Shared.Business;

public class ConnectedBot
{
    public long BotId { get; set; }
    public List<long> Recipients { get; set; } = new();
    public bool CanReply { get; set; }
}
