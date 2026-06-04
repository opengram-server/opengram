namespace MyTelegram.Domain.CommandHandlers.Temp;

public class StartDeleteChannelMessagesCommandHandler : CommandHandler<TempAggregate, TempId, StartDeleteChannelMessagesCommand>
{
    public override Task ExecuteAsync(TempAggregate aggregate, StartDeleteChannelMessagesCommand command, CancellationToken cancellationToken)
    {
        aggregate.StartDeleteChannelMessages(command.RequestInfo, command.ChannelId, command.MessageIds, command.NewTopMessageId, command.NewTopMessageIdForDiscussionGroup, command.DiscussionGroupChannelId, command.RepliesMessageIds);

        return Task.CompletedTask;
    }
}