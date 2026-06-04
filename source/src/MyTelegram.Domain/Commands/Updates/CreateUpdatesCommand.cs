using MyTelegram.Domain.Aggregates.Updates;

namespace MyTelegram.Domain.Commands.Updates;

public class CreateUpdatesCommand(
    UpdatesId aggregateId,
    long ownerPeerId,
    long? excludeAuthKeyId,
    long? excludeUserId,
    long? onlySendToUserId,
    long? onlySendToThisAuthKeyId,
    UpdatesType updatesType,
    int pts,
    int? messageId,
    int date,
    long seqNo,
    IList<IUpdate>? updates,
    List<long>? users,
    List<long>? chats)
    : Command<UpdatesAggregate, UpdatesId, IExecutionResult>(aggregateId)
{
    public long OwnerPeerId { get; } = ownerPeerId;

    //public long? ChannelId { get; }
    public long? ExcludeAuthKeyId { get; } = excludeAuthKeyId;
    public long? ExcludeUserId { get; } = excludeUserId;
    public long? OnlySendToUserId { get; } = onlySendToUserId;
    public long? OnlySendToThisAuthKeyId { get; } = onlySendToThisAuthKeyId;
    public UpdatesType UpdatesType { get; } = updatesType;
    public int Pts { get; } = pts;
    public int? MessageId { get; } = messageId;
    public int Date { get; } = date;
    public long SeqNo { get; } = seqNo;
    public IList<IUpdate>? Updates { get; } = updates;

    public List<long>? Users { get; } = users;
    public List<long>? Chats { get; } = chats;

    /*long reqMsgId,*/
    //long? channelId,
    //ChannelId = channelId;
}
