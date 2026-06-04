namespace MyTelegram.Domain.CommandHandlers.Channel;

public class ReadChannelLatestNonBotOutboxMessageCommandHandler : CommandHandler<ChannelAggregate, ChannelId,
    ReadChannelLatestNonBotOutboxMessageCommand>
{
    public override Task ExecuteAsync(ChannelAggregate aggregate,
        ReadChannelLatestNonBotOutboxMessageCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.ReadChannelLatestNonBotOutboxMessage(command.RequestInfo, command.SourceCommandId);
        return Task.CompletedTask;
    }
}
