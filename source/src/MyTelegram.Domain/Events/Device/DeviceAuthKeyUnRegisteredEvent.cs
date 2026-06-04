namespace MyTelegram.Domain.Events.Device;

public class DeviceAuthKeyUnRegisteredEvent(
    long permAuthKeyId,
    long tempAuthKeyId) : AggregateEvent<DeviceAggregate, DeviceId>
{
    public long PermAuthKeyId { get; } = permAuthKeyId;
    public long TempAuthKeyId { get; } = tempAuthKeyId;
}
