namespace MyTelegram.Queries.Privacy;

public class GetGlobalPrivacySettingsQuery(long userId) : IQuery<GlobalPrivacySettings?>
{
    public long UserId { get; } = userId;
}
