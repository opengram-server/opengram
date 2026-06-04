namespace MyTelegram.Domain.CommandHandlers.Channel;

public class
    HideChatJoinRequestCommand2Handler : CommandHandler<JoinChannelAggregate, JoinChannelId,
    HideChatJoinRequestCommand2>
{
    public override Task ExecuteAsync(JoinChannelAggregate aggregate, HideChatJoinRequestCommand2 command,
        CancellationToken cancellationToken)
    {
        aggregate.HideChatJoinRequest(command.RequestInfo, command.UserId, command.Approved, command.TopMessageId, command.ChannelHistoryMinId, command.Broadcast);

        return Task.CompletedTask;
    }
}