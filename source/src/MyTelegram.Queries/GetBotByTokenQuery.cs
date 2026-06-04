namespace MyTelegram.Queries;

public class GetBotByTokenQuery(string token) : IQuery<IBotReadModel?>
{
    public string Token { get; } = token;
}
