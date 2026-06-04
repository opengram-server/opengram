namespace MyTelegram.Domain.Events.Device;

public class DeviceRegisteredEvent(
    RequestInfo requestInfo,
    int tokenType,
    string token)
    : RequestAggregateEvent2<DeviceAggregate, DeviceId>(requestInfo)
{
    public string Token { get; } = token;
    public int TokenType { get; } = tokenType;
}
