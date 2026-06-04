namespace MyTelegram.ReadModel.Interfaces;

public interface IChannelMemberReadModel : IReadModel
{
    int BannedRights { get; }
    long ChannelId { get; }
    int Date { get; }
    string Id { get; }
    // ReSharper disable once IdentifierTypo
    long InviterId { get; }
    bool IsBot { get; }
    bool Kicked { get; }
    long KickedBy { get; }
    bool Left { get; }
    int UntilDate { get; }
    long UserId { get; }
    long? ChatInviteId { get; }
    ChatJoinType ChatJoinType { get; }
    int? SubscriptionUntilDate { get; }
    bool? IsBroadcast { get; }
    bool IsAdmin { get; }
    //bool IsCreator { get; }
    string? Rank { get; }
    bool CanEdit { get; }
    long? PromotedBy { get; }
    int AdminRights { get; }
}