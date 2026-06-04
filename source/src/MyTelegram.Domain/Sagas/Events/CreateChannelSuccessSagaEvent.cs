namespace MyTelegram.Domain.Sagas.Events;

public class CreateChannelSuccessSagaEvent(
    long reqMsgId,
    long selfAuthKeyId,
    long channelId,
    long creatorUid,
    int date,
    string messageActionData,
    long randomId)
    : AggregateEvent<CreateChannelSaga, CreateChannelSagaId>
{
    public long ChannelId { get; } = channelId;
    public long CreatorUid { get; } = creatorUid;
    public int Date { get; } = date;
    public string MessageActionData { get; } = messageActionData;
    public long RandomId { get; } = randomId;
    public long ReqMsgId { get; } = reqMsgId;
    public long SelfAuthKeyId { get; } = selfAuthKeyId;
}
