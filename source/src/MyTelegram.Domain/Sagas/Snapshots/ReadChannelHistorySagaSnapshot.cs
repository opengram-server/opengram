namespace MyTelegram.Domain.Sagas.Snapshots;

public class ReadChannelHistorySagaSnapshot(
    long reqMsgId,
    long readerUid,
    long channelId,
    Guid correlationId)
    : ISnapshot
{
    public long ChannelId { get; } = channelId;
    public Guid CorrelationId { get; } = correlationId;
    public long ReaderUid { get; } = readerUid;

    public long ReqMsgId { get; } = reqMsgId;
}
