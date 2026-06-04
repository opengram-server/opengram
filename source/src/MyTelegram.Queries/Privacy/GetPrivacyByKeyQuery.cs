namespace MyTelegram.Queries.Privacy;

public class GetPrivacyByKeyQuery(MyTelegram.Schema.IInputPrivacyKey key, long? phone = null) : IQuery<IReadOnlyCollection<IPrivacyReadModel>>
{
    public MyTelegram.Schema.IInputPrivacyKey Key { get; } = key;
    public long? Phone { get; } = phone;
}
