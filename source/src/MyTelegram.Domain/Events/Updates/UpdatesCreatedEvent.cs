using MyTelegram.Domain.Aggregates.Updates;

namespace MyTelegram.Domain.Events.Updates;

public class UpdatesCreatedEvent(
    long ownerPeerId,
    long? excludeAuthKeyId,
    long? excludeUserId,
    long? onlySendToUserId,
    long? onlySendToThisAuthKeyId,
    UpdatesType updatesType,
    int pts,
    int? messageId,
    int date,
    long globalSeqNo,
    IList<IUpdate>? updates,
    List<long>? users,
    List<long>? chats)
    : AggregateEvent<UpdatesAggregate, UpdatesId>
{
    //long channelId,
    //ChannelId = channelId;

    public long OwnerPeerId { get; } = ownerPeerId;

    //public long ChannelId { get; }
    public long? ExcludeAuthKeyId { get; } = excludeAuthKeyId;
    public long? ExcludeUserId { get; } = excludeUserId;
    public long? OnlySendToUserId { get; } = onlySendToUserId;
    public long? OnlySendToThisAuthKeyId { get; } = onlySendToThisAuthKeyId;
    public UpdatesType UpdatesType { get; } = updatesType;
    public int Pts { get; } = pts;
    public int? MessageId { get; } = messageId;
    public int Date { get; } = date;
    public long GlobalSeqNo { get; } = globalSeqNo;
    public IList<IUpdate>? Updates { get; } = updates;
    public List<long>? Users { get; } = users;
    public List<long>? Chats { get; } = chats;
}