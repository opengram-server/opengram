namespace MyTelegram.Domain.CommandHandlers.Temp;

public class
    StartSetDiscussionGroupCommandHandler : CommandHandler<TempAggregate, TempId, StartSetDiscussionGroupCommand>
{
    public override Task ExecuteAsync(TempAggregate aggregate, StartSetDiscussionGroupCommand command, CancellationToken cancellationToken)
    {
        aggregate.StartSetChannelDiscussionGroup(command.RequestInfo, command.BroadcastChannelId, command.DiscussionGroupChannelId);
        return Task.CompletedTask;
    }
}