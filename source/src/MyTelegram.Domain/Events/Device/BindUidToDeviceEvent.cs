namespace MyTelegram.Domain.Events.Device;

public class BindUidToDeviceEvent(
    long userId,
    long permAuthKeyId,
    int date) : AggregateEvent<DeviceAggregate, DeviceId>
{
    public int Date { get; } = date;
    public long PermAuthKeyId { get; } = permAuthKeyId;

    public long UserId { get; } = userId;
}
