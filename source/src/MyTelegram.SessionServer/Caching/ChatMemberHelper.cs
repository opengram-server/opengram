using System.Collections.Concurrent;

namespace MyTelegram.SessionServer.Caching;

public sealed class ChatMemberHelper : IChatMemberHelper
{
    private readonly ConcurrentDictionary<long, ConcurrentDictionary<long, byte>> _members = new();

    public void SetMembers(long chatId, IReadOnlyList<long> memberUserIds)
    {
        var dict = new ConcurrentDictionary<long, byte>();
        foreach (var uid in memberUserIds)
            dict[uid] = 1;
        _members[chatId] = dict;
    }

    public IReadOnlyList<long>? GetMembers(long chatId)
    {
        if (_members.TryGetValue(chatId, out var dict))
            return dict.Keys.ToList();
        return null;
    }

    public void RemoveMember(long chatId, long userId)
    {
        if (_members.TryGetValue(chatId, out var dict))
            dict.TryRemove(userId, out _);
    }

    public void AddMember(long chatId, long userId)
    {
        var dict = _members.GetOrAdd(chatId, _ => new ConcurrentDictionary<long, byte>());
        dict[userId] = 1;
    }
}
