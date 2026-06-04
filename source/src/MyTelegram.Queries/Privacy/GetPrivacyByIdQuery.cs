namespace MyTelegram.Queries.Privacy;

public class GetPrivacyByIdQuery(string privacyId) : IQuery<IPrivacyReadModel?>
{
    public string PrivacyId { get; } = privacyId;
}
