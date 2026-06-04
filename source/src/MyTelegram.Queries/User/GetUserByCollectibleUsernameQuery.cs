namespace MyTelegram.Queries.User;

public class GetUserByCollectibleUsernameQuery(string username) : IQuery<IUserReadModel?>
{
    public string Username { get; } = username;
}
