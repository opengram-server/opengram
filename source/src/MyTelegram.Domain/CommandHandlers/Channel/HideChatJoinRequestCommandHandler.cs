namespace MyTelegram.Domain.CommandHandlers.Channel;

public class
    HideChatJoinRequestCommandHandler : CommandHandler<JoinChannelAggregate, JoinChannelId,
    HideChatJoinRequestCommand>
{
    public override Task ExecuteAsync(JoinChannelAggregate aggregate, HideChatJoinRequestCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.HideChatJoinRequest(command.RequestInfo, command.UserId, command.Approved, command.TopMessageId, command.ChannelHistoryMinId, command.Broadcast);

        return Task.CompletedTask;
    }
}