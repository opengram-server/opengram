namespace MyTelegram.Domain.CommandHandlers.Channel;

public class SetChannelHistoryTTLCommandHandler : CommandHandler<ChannelAggregate, ChannelId, SetChannelHistoryTTLCommand>
{
    public override Task ExecuteAsync(ChannelAggregate aggregate, SetChannelHistoryTTLCommand command, CancellationToken cancellationToken)
    {
        aggregate.SetHistoryTTL(command.RequestInfo, command.TtlPeriod);
        return Task.CompletedTask;
    }
}
