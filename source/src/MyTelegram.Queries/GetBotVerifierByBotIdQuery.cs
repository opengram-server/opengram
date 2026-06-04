namespace MyTelegram.Queries;

public class GetBotVerifierByBotIdQuery : IQuery<IBotVerifierReadModel?>
{
    public GetBotVerifierByBotIdQuery(long botUserId)
    {
        BotUserId = botUserId;
    }

    public long BotUserId { get; }
}
