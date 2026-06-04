namespace MyTelegram.Queries.Privacy;

public class GetPrivacyByUserIdQuery(long userId, MyTelegram.Schema.IInputPrivacyKey? key = null) : IQuery<IReadOnlyCollection<IPrivacyReadModel>>
{
    public long UserId { get; } = userId;
    public MyTelegram.Schema.IInputPrivacyKey? Key { get; } = key;
}
