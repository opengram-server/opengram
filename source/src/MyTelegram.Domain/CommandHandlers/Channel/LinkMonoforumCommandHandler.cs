namespace MyTelegram.Domain.CommandHandlers.Channel;

public class LinkMonoforumCommandHandler : CommandHandler<ChannelAggregate, ChannelId, LinkMonoforumCommand>
{
    public override Task ExecuteAsync(ChannelAggregate aggregate, LinkMonoforumCommand command, CancellationToken cancellationToken)
    {
        aggregate.LinkMonoforum(
            command.RequestInfo, 
            command.LinkedMonoforumId, 
            command.IsMonoforum,
            command.BroadcastMessagesAllowed);

        return Task.CompletedTask;
    }
}
