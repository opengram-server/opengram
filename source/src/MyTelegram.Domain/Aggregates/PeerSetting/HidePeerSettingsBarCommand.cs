using MyTelegram.Domain.Commands;

namespace MyTelegram.Domain.Aggregates.PeerSetting;

public class HidePeerSettingsBarCommand(PeerSettingsId aggregateId, RequestInfo requestInfo, long peerId)
    : RequestCommand2<PeerSettingsAggregate, PeerSettingsId, IExecutionResult>(aggregateId, requestInfo)
{
    public long PeerId { get; } = peerId;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(RequestInfo.ReqMsgId);
        yield return RequestInfo.RequestId.ToByteArray();
    }
}