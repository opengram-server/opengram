namespace MyTelegram.Queries.Privacy;

public class GetUserPrivacyListQuery(long userId) : IQuery<IReadOnlyCollection<IPrivacyReadModel>>
{
    public long UserId { get; } = userId;
}
