namespace MyTelegram.Queries;

public class GetUserPasswordQuery : IQuery<IUserPasswordReadModel?>
{
    public GetUserPasswordQuery(long userId)
    {
        UserId = userId;
    }

    public long UserId { get; }
}
