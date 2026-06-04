using MyTelegram.Domain.Aggregates.GroupCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Commands.GroupCall;

public class JoinGroupCallCommand(
    GroupCallId aggregateId,
    RequestInfo requestInfo,
    long peerId,
    PeerType peerType,
    int source,
    bool muted,
    bool videoStopped,
    string? params_,
    int date)
    : RequestCommand2<GroupCallAggregate, GroupCallId, IExecutionResult>(aggregateId, requestInfo)
{
    public long PeerId { get; } = peerId;
    public PeerType PeerType { get; } = peerType;
    public int Source { get; } = source;
    public bool Muted { get; } = muted;
    public bool VideoStopped { get; } = videoStopped;
    public string? Params { get; } = params_;
    public int Date { get; } = date;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(PeerId);
        yield return BitConverter.GetBytes(Source);
    }
}
