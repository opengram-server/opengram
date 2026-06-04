namespace MyTelegram.Core;
public record ChannelMemberChangedEvent(
    long ChannelId,
    MemberStateChangeType MemberStateChangeType,
    IReadOnlyList<long> MemberUidList);