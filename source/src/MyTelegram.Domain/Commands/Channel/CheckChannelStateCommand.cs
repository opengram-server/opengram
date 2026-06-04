namespace MyTelegram.Domain.Commands.Channel;

public class CheckChannelStateCommand(
    ChannelId aggregateId,
    RequestInfo requestInfo,
    long senderPeerId,
    int messageId,
    int date,
    MessageSubType messageSubType)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public long SenderPeerId { get; } = senderPeerId;
    public int MessageId { get; } = messageId;
    public int Date { get; } = date;
    public MessageSubType MessageSubType { get; } = messageSubType;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return RequestInfo.RequestId.ToByteArray();
    }
}