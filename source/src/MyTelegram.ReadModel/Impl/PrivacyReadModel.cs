namespace MyTelegram.ReadModel.Impl;

public partial class PrivacyReadModel : IPrivacyReadModel
{
    public virtual string Id { get; set; } = default!;
    public virtual long? Version { get; set; }
    public long UserId { get; set; }
    public PrivacyType PrivacyType { get; set; }
    public IReadOnlyList<PrivacyValueData> PrivacyValueDataList { get; set; } = new List<PrivacyValueData>();
}
