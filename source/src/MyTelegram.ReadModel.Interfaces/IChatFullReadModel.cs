using MyTelegram.Domain;

namespace MyTelegram.ReadModel.Interfaces;

public interface IChatFullReadModel : IReadModel
{
    long ChatId { get; }
    string? About { get; }
    int? PinnedMsgId { get; }
    int? FolderId { get; }
    int? TtlPeriod { get; }
    ReactionType ReactionType { get; }
    bool AllowCustomReaction { get; }
    List<string>? AvailableReactions { get; }
    int? RequestsPending { get; }
    List<long>? RecentRequesters { get; }
    
    // Group call
    long? ActiveGroupCallId { get; }
}
