namespace MyTelegram.Domain.CommandHandlers.Channel;

public class
    CreateJoinChannelRequestCommandHandler : CommandHandler<JoinChannelAggregate, JoinChannelId,
    CreateJoinChannelRequestCommand>
{
    public override Task ExecuteAsync(JoinChannelAggregate aggregate, CreateJoinChannelRequestCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Create(command.RequestInfo, command.ChannelId, command.RequestInfo.UserId, command.InviteId);

        return Task.CompletedTask;
    }
}