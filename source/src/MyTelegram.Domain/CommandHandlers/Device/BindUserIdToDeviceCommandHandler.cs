namespace MyTelegram.Domain.CommandHandlers.Device;

public class BindUserIdToDeviceCommandHandler : CommandHandler<DeviceAggregate, DeviceId, BindUserIdToDeviceCommand>
{
    public override Task ExecuteAsync(DeviceAggregate aggregate,
        BindUserIdToDeviceCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.BindUserIdToDevice(command.UserId, command.PermAuthKeyId);
        return Task.CompletedTask;
    }
}
