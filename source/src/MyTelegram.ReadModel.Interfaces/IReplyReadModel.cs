namespace MyTelegram.ReadModel.Interfaces;

public interface IReplyReadModel : IReadModel
{
    long ChannelId { get; }
    int MessageId { get; }
    int Replies { get; }
    int RepliesPts { get; }
    IReadOnlyCollection<Peer>? RecentRepliers { get; }
    int? MaxId { get; }
    long? CommentChannelId { get; }
}
