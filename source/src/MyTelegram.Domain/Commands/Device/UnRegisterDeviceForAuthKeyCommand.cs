namespace MyTelegram.Domain.Commands.Device;

public class UnRegisterDeviceForAuthKeyCommand(
    DeviceId aggregateId,
    long permAuthKeyId,
    long tempAuthKeyId)
    : Command<DeviceAggregate, DeviceId, IExecutionResult>(aggregateId)
{
    public long PermAuthKeyId { get; } = permAuthKeyId;
    public long TempAuthKeyId { get; } = tempAuthKeyId;
}
