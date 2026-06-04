using MyTelegram.Domain.Aggregates.PhoneCall;
using MyTelegram.Domain.Shared;

namespace MyTelegram.Domain.Commands.PhoneCall;

public class RequestCallCommand(
    PhoneCallId aggregateId,
    RequestInfo requestInfo,
    long callId,
    long accessHash,
    long adminId,
    long participantId,
    bool isVideo,
    byte[] gAHash,
    PhoneCallProtocol protocol,
    int date)
    : RequestCommand2<PhoneCallAggregate, PhoneCallId, IExecutionResult>(aggregateId, requestInfo)
{
    public long CallId { get; } = callId;
    public long AccessHash { get; } = accessHash;
    public long AdminId { get; } = adminId;
    public long ParticipantId { get; } = participantId;
    public bool IsVideo { get; } = isVideo;
    public byte[] GAHash { get; } = gAHash;
    public PhoneCallProtocol Protocol { get; } = protocol;
    public int Date { get; } = date;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(CallId);
    }
}
