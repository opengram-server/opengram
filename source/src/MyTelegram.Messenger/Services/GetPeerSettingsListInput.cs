namespace MyTelegram.Messenger.Services;

public class GetPeerSettingsListInput(
    long userId,
    List<long> targetUserIdList)
{
    public List<long> TargetUserIdList { get; } = targetUserIdList;

    public long UserId { get; } = userId;
}