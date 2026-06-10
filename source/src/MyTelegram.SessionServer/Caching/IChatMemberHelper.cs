namespace MyTelegram.SessionServer.Caching;

/// <summary>
/// Caches chat/channel member lists for push notification targeting.
/// Mirrors the original binary's ChatMemberHelper.
/// </summary>
public interface IChatMemberHelper
{
    void SetMembers(long chatId, IReadOnlyList<long> memberUserIds);
    IReadOnlyList<long>? GetMembers(long chatId);
    void RemoveMember(long chatId, long userId);
    void AddMember(long chatId, long userId);
}
