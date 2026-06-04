namespace MyTelegram.ReadModel.Interfaces;

public interface IPrivacyReadModel : IReadModel
{
    string Id { get; }
    PrivacyType PrivacyType { get; }
    IReadOnlyList<PrivacyValueData> PrivacyValueDataList { get; }
    long UserId { get; }
}