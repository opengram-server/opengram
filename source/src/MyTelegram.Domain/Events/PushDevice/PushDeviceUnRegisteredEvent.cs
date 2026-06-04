namespace MyTelegram.Domain.Events.PushDevice;

public class PushDeviceUnRegisteredEvent(
    RequestInfo requestInfo,
    int tokenType,
    string token,
    IReadOnlyList<long> otherUids)
    : RequestAggregateEvent2<PushDeviceAggregate, PushDeviceId>(requestInfo)
{
    public IReadOnlyList<long> OtherUids { get; } = otherUids;
    public string Token { get; } = token;
    public int TokenType { get; } = tokenType;
}
