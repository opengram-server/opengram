namespace MyTelegram.Core;

public record ChatMemberChangedEvent(
    long ChatId,
    MemberStateChangeType MemberStateChangeType,
    IReadOnlyCollection<long> MemberUidList);