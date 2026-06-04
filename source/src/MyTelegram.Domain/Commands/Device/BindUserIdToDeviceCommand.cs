namespace MyTelegram.Domain.Commands.Device;

public class BindUserIdToDeviceCommand(
    DeviceId aggregateId,
    long userId,
    long permAuthKeyId)
    : Command<DeviceAggregate, DeviceId, IExecutionResult>(aggregateId)
{
    public long PermAuthKeyId { get; } = permAuthKeyId;

    public long UserId { get; } = userId;
}
