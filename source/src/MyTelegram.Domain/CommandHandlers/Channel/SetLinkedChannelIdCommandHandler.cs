namespace MyTelegram.Domain.CommandHandlers.Channel;

public class SetLinkedChannelIdCommandHandler : CommandHandler<ChannelAggregate, ChannelId, SetLinkedChannelIdCommand>
{
    public override Task ExecuteAsync(ChannelAggregate aggregate, SetLinkedChannelIdCommand command, CancellationToken cancellationToken)
    {
        aggregate.SetLinkedChannelId(command.RequestInfo, command.LinkedChannelId);
        return Task.CompletedTask;
    }
}